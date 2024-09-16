using SabaSingleWallet.API.Messages;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services)
		=> services
		.AddTransient<ISapaApiMessagePublishHandler, SapaApiMessagePublishHandler>();
}