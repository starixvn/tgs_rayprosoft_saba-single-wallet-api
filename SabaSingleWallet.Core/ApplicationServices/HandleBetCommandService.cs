using Microsoft.Extensions.Logging;
using SabaSingleWallet.Core.Models;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.Core.ApplicationServices;

public class HandleBetCommandService(ILogger<HandleBetCommandService> logger) : ICommandService<HandleBetCommand>
{
	public async ValueTask ExecuteAsync(HandleBetCommand command, CancellationToken cancellationToken = default)
	{
		Guid gameProviderId;
		PlayerInfo playerInfo;
		Guid gameProviderConfigurationId;
		GameInfo gameInfo;
	}
}