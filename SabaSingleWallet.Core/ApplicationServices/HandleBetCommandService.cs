using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.Core.ApplicationServices;

public class HandleBetCommandService(ILogger<HandleBetCommandService> logger, IGameCalculate gameCalculate) : ICommandService<HandleBetCommand>
{
	public async ValueTask ExecuteAsync(HandleBetCommand command, CancellationToken cancellationToken = default)
	{
		var bet = gameCalculate.Execute(command);
		var x = "stop";
	}
}