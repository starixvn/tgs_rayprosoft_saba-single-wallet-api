using MongoDB.Driver;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApiLogsRepositories(
		this IServiceCollection services,
		Func<IServiceProvider, IMongoClient> mongoClientResolver)
		=> services
		.AddTransient<IApiLogsRepository>((sp) => ActivatorUtilities.CreateInstance<ApiLogsRepository>(sp, mongoClientResolver(sp)));

	public static IServiceCollection AddPlayerRepository(this IServiceCollection services)
		   => services.AddScoped<IPlayerRepository, PlayerRepositoryImp>();
}