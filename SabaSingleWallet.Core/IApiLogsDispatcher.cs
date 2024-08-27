using SabaSingleWallet.Core.ApplicationServices;

namespace SabaSingleWallet.Core;

public interface IApiLogsDispatcher
{
	ValueTask PushCommandAsync(ApiLogsCommand command, CancellationToken cancellationToken = default);
}