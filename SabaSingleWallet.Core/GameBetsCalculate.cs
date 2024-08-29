using System.Text.Json;
using Microsoft.Extensions.Options;
using SabaSingleWallet.Core.ApplicationServices;
using SabaSingleWallet.Core.Models;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaSingleWallet.Core;

public interface IGameCalculate
{
	GameBetHandle Execute(HandleBetCommand command);
}

internal class GameCalculate(IOptions<GameProviderInfo> gameRecordInfoOptions) : IGameCalculate
{
	private readonly GameProviderInfo _gameRecordInfoOptions = gameRecordInfoOptions.Value;

	public GameBetHandle Execute(HandleBetCommand command)
	{
		var betState = command.BetState;
		var isClosed = betState is not BetState.Running;
		var idVoid = betState is BetState.Void or BetState.Running or BetState.Refund or BetState.Refund;
		var isCashOut = command.ExtraStatus.Equals("cashout", StringComparison.OrdinalIgnoreCase);

		var bet = command.BetAmount;
		var validBet = isCashOut ? 0 : !idVoid ? command.BetAmount : 0;

		var settle = 0.0m;
		var turnover = 0.0m;
		var winLoss = 0.0m;
		var cashoutState = CashoutState.None;
		var externalClosedOn = DateTimeOffset.UnixEpoch.DateTime;

		if (isClosed)
		{
			turnover = command.BetAmount;
			settle = command.Settle;
			externalClosedOn = command.WinlostDate;
			winLoss = command.ActualAmount;
			cashoutState = isCashOut ? CashoutState.Cashout : CashoutState.None;
		}

		return new GameBetHandle
		{
			Action = command.Action,
			GameProviderCode = _gameRecordInfoOptions.GameProviderCode,
			ExternalGameId = _gameRecordInfoOptions.GameExternalId,
			PlayerCode = command.PlayerCode,
			ExternalBetId = command.TxId,
			ExternalRoundId = command.TxId,
			ExternalCreatedOn = command.TransactionTime,
			ExternalClosedOn = externalClosedOn,
			ExternalPayoffOn = externalClosedOn,
			ExternalResettleOn = DateTimeOffset.UnixEpoch.DateTime,
			BetState = betState,
			BetType = BetType.Normal,
			FreeGameState = FreeGameState.None,
			GambleState = GambleState.None,
			CashoutState = cashoutState,
			ResettleState = ResettleState.None,
			JackpotState = JackpotState.None,
			Bet = bet,
			ValidBet = validBet,
			Settle = settle,
			Commission = 0.0m,
			Turnover = turnover,
			WinLoss = winLoss,
			Jackpot = 0.0M,
			AdditionalData = JsonSerializer.Serialize(command)
		};
	}
}