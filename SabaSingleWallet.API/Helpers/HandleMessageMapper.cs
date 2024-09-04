using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.Settle;
using SabaSingleWallet.Repositories.Models.Unsettle;
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
			Payout = data.BetAmount,
			ActualAmount = data.BetAmount,
		};
	}

	public static HandleBetMessage ConfirmBet(ConfirmBetCriteria data, string PlayerCode)
	{
		return new HandleBetMessage()
		{
			Action = "ConfirmBet",
			PlayerCode = PlayerCode,
			BetState = (int)BetState.Running,
			TxId = data.TxId.ToString(),
			BetAmount = data.ActualAmount,
			Payout = data.ActualAmount,
			TransactionTime = data.TransactionTime,
			ActualAmount = data.ActualAmount,
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

	public static HandleBetMessage Settle(SettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Settle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = (int)StatusToInt(data.Status),
			TxId = data.TxId.ToString(),
			Payout = data.Payout,
			Settle = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate?.Date ?? DateTimeOffset.UnixEpoch.DateTime,
		};
	}

	public static HandleBetMessage Unsettle(UnsettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Unsettle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = (int)StatusToInt(data.Status),
			TxId = data.TxId.ToString(),
			Payout = data.Payout,
			Settle = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate?.Date ?? DateTimeOffset.UnixEpoch.DateTime,
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
			Payout = data.Payout,
			Settle = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate?.Date ?? DateTimeOffset.UnixEpoch.DateTime,
		};
	}

	private static BetState StatusToInt(string status)
	{
		return status.ToLower() switch
		{
			"won" => BetState.Win,
			"lost" => BetState.Loss,
			"draw" => BetState.Draw,
			"void" => BetState.Void,
			"refund" => BetState.Refund,
			"reject" => BetState.Cancel,
			"half won" => BetState.HalfWin,
			"half lost" => BetState.HalfLoss,
			_ => BetState.Running,
		};
	}
}