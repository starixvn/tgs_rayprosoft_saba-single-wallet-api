using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Models.CancelBet;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.GameInterface.SabaService.Protos;
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
			WinLoss = data.BetAmount,
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
			WinLoss = data.ActualAmount,
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

	public static HandleBetMessage Resettle(SettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Resettle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = (int)StatusToInt(data.Status),
			TxId = data.TxId.ToString(),
			WinLoss = data.Payout,
			TransactionTime = DateTime.Now,
			Settle = data.Payout,
			ExtraStatus = data.ExtraStatus,
			WinlostDate = data.WinlostDate?.Date ?? DateTimeOffset.UnixEpoch.DateTime,
		};
	}

	private static BetState StatusToInt(string status)
	{
		switch (status.ToLower())
		{
			case "won":
				return BetState.Win;
			case "lost":
				return BetState.Loss;
			case "draw":
				return BetState.Draw;
			case "void":
				return BetState.Void;
			case "refund":
				return BetState.Refund;
			case "reject":
				return BetState.Cancel;
			case "half won":
				return BetState.HalfWin;
			case "half lost":
				return BetState.HalfLoss;
			default:
				return BetState.Running;
		}
	}
}