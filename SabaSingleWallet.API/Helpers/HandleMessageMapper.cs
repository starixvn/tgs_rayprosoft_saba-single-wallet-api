using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Models.CancelBet;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaSingleWallet.API.Helpers;

public class HandleMessageMapper
{
	public static HandleBetMessage PlaceBet(PlaceBetCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "PlaceBet",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = (int)BetState.Waiting,
			TxId = data.TxId.ToString(),
			BetAmount = data.BetAmount,
			ActualAmount = data.BetAmount,
			TransactionTime = DateTime.Now,
		};
	}

	public static HandleBetMessage ConfirmBet(ConfirmBetCriteria data, string PlayerCode)
	{
		return new HandleBetMessage()
		{
			Action = "ConfirmBet",
			PlayerCode = PlayerCode,
			BetState = (int)BetState.Waiting,
			TxId = data.TxId.ToString(),
			BetAmount = data.ActualAmount,
			ActualAmount = data.ActualAmount,
			TransactionTime = data.TransactionTime,
		};
	}

	public static HandleBetMessage CancelBet(string PlayerCode)
	{
		return new HandleBetMessage()
		{
			Action = "CancelBet",
			PlayerCode = PlayerCode,
			BetState = (int)BetState.Cancel,
		};
	}
}