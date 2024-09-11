using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.Resettle;
using SabaSingleWallet.Repositories.Models.Settle;
using SabaSingleWallet.Repositories.Models.Unsettle;
using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaSingleWallet.API.Helpers;

public static class HandleMessageMapper
{
	public static HandleBetMessage ConfirmBet(ConfirmBetCriteria data, string PlayerCode)
	{
		return new HandleBetMessage()
		{
			Action = "ConfirmBet",
			PlayerCode = PlayerCode,
			TxId = data.TxId.ToString(),
			LicenseeTxid = data.LicenseetxId,
			TransactionTime = data.TransactionTime,
		};
	}

	public static HandleBetMessage Settle(SettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Settle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = data.Status,
			TxId = data.TxId.ToString(),
			RefId = data.RefId,
			Payout = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate?.DateTime,
		};
	}

	public static HandleBetMessage Unsettle(UnsettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Unsettle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = data.Status,
			TxId = data.TxId.ToString(),
			Payout = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate,
		};
	}

	public static HandleBetMessage Resettle(ResettleCriteria data)
	{
		return new HandleBetMessage()
		{
			Action = "Resettle",
			PlayerCode = data.UserId.ToPlayerCode(),
			BetState = data.Status,
			TxId = data.TxId.ToString(),
			Payout = data.Payout,
			ExtraStatus = data.ExtraStatus ?? "",
			WinlostDate = data.WinlostDate.DateTime,
			IsOnlyWinlostDateChanged = data.IsOnlyWinlostDateChanged,
		};
	}
}