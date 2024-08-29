using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Messages;

public interface ISapaApiMessagePublishHandler
{
	ValueTask LogApi(ApiLogMessage mesage, CancellationToken cancellationToken = default);

	ValueTask HandleBetAsync(HandleBetMessage mesage, CancellationToken cancellationToken = default);
}