using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	//public static IServiceCollection AddGameRecordRepositories(
	//	this IServiceCollection services,
	//	Func<IServiceProvider, IMongoClient> mongoClientResolver)
	//	=> services
	//	.AddTransient<IGameRecordScheduleRepository>((sp) => ActivatorUtilities.CreateInstance<GameRecordScheduleRepository>(sp, mongoClientResolver(sp)));
}
