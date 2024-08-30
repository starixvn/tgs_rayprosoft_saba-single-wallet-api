using Microsoft.Extensions.Logging;
using SabaSingleWallet.Core.Models;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.Core.ApplicationServices;

public class HandleBetCommandService(ILogger<HandleBetCommandService> logger) : ICommandService<HandleBetCommand>
{
	public async ValueTask ExecuteAsync(HandleBetCommand command, CancellationToken cancellationToken = default)
	{
		if (command.Action == "CancelBet")
		{
		}
		Guid gameProviderId;
		PlayerInfo playerInfo;
		Guid gameProviderConfigurationId;
		GameInfo gameInfo;
		GameCurrencyInfo gameCurrencyInfo;
		Guid gameRoundId;

		// TODO call SP create Bet
	}
}