using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SabaSingleWallet.Core.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddSabaSingleWalletApiCore(
	this IServiceCollection services)
	=> services
	.RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
	.AddSerialNumberGenerator();
}