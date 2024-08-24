using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SabaSingleWallet.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameRecordRepositories(
		this IServiceCollection services,
		Func<IServiceProvider, IMongoClient> mongoClientResolver)
		=> services
		.AddTransient<IPlaceBetLogsRepository>((sp) => ActivatorUtilities.CreateInstance<PlaceBetLogsRepository>(sp, mongoClientResolver(sp)));
}
