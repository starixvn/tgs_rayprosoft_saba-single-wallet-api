using SabaSingleWallet.Core.ApplicationServices;

namespace SabaSingleWallet.Core;

public interface IHandleBetDispatcher
{
	ValueTask PushCommandAsync(HandleBetCommand command, CancellationToken cancellationToken = default);
}