using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using SabaSingleWallet.Core;

namespace SabaSingleWallet.Repositories.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRepositories(this IServiceCollection services)
		   => services.AddScoped<IDynamicRepository, DynamicRepository>()
					  .AddScoped<IFanexPlayerRepository, FanexPlayerRepositoryImp>()
					  .AddScoped<IPlayerRepository, PlayerRepositoryImp>();
}