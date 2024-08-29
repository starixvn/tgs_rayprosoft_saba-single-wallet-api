using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Models.CancelBet;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaSingleWallet.API.Helpers;

public class MapActionToHandleBetMessage
{
	public static HandleBetMessage PlaceBet(PlaceBetCriteria data)
	{
		return new HandleBetMessage()
		{
			PlayerCode = "4ulmnvs1skex",
			BetState = (int)BetState.Waiting,
			TxId = "BG-592664351",
			BetAmount = 69696969,
			ActualAmount = 696969696,
			TransactionTime = DateTime.Now,
		};
	}

	public static HandleBetMessage ConfirmBet(ConfirmBetCriteria data, string PlayerCode)
	{
		return new HandleBetMessage()
		{
			PlayerCode = PlayerCode,
			BetState = (int)BetState.Waiting,
			TxId = data.TxId.ToString(),
			BetAmount = data.ActualAmount,
			ActualAmount = data.ActualAmount,
			TransactionTime = data.TransactionTime,
		};
	}

	public static HandleBetMessage CancelBet(CancelBetCriteria data, string PlayerCode)
	{
		return new HandleBetMessage()
		{
			PlayerCode = PlayerCode,
			BetState = (int)BetState.Cancel,
		};
	}
}