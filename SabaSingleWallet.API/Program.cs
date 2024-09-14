using Microsoft.AspNetCore.HttpLogging;
using NATS.Client.Core;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SabaSingleWallet.API;
using SabaSingleWallet.Core.DependencyInjection;
using SabaSingleWallet.Repositories.DependencyInjection;
using Tgs.GameRecord.Saba.Common;
using Tgs.MessageQueue.Nats.Serializers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddSabaSingleWalletApiCore()
	.AddRepositories()
	.AddCashServiceClient((op, sp) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		op.BaseUrl = config.GetValue<Uri>("SabaSingleWalletConnectionStrings:CashClientBaseUrl")!;
	});

builder.Services
.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
{
	Url = builder.Configuration.GetConnectionString("Nats")!,
	SerializerRegistry = TgsNatsSerializerRegistration.Default
}))
.AddMessageQueue()
.AddNatsMessageQueue(config => config.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats")))
 .ClearAllExchanges()
 .AddNatsGlobPatternExchange(SabaNatsConst.ApiLogsPublish)
 .AddNatsGlobPatternExchange(SabaNatsConst.HandleBetPublish);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGameInterfaceCore();
builder.Services
	.AddHttpLogging(logging => logging.LoggingFields = HttpLoggingFields.All)
	.AddHealthChecks();
builder.WebHost.UseSentry();

var excludeExtensions = new[] { "html", "ico", "png", "jpg" };

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName") ?? ""))
	.WithTracing(tracerProviderBuilder => tracerProviderBuilder
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation(options => options.Filter =
			httpContext => !httpContext.Request.Path.HasValue
				|| !httpContext.Request.Path.StartsWithSegments("/swagger")
				&& !httpContext.Request.Path.StartsWithSegments("/health")
				&& !httpContext.Request.Path.StartsWithSegments("/healthcheck")
				&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
				&& !httpContext.Request.Path.StartsWithSegments("/metrics")
				&& !excludeExtensions.Any(
					ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.SetSampler(new AlwaysOnSampler())
		.AddNpgsql()
		.AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector") ?? new Uri("")))
	.WithMetrics(meterBuiler => meterBuiler
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
			.AddRuntimeInstrumentation()
			.AddAspNetCoreInstrumentation()
			.AddOtlpExporter())
	.Services
	.AddProfileViewer()
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddRequestDecompression();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestDecompression();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapHealthChecks("/healthcheck");

app.MapHealthChecks("/");

app.Run();