using Tgs.GameRecord.Handle.Common.Protos;

namespace SabaSingleWallet.Core.ApplicationServices;

public record HandleBetCommand
{
	public string Action { get; set; } = "";

	public string PlayerCode { get; set; } = "";

	public string ExtraStatus { get; set; } = "";

	public BetState BetState { get; set; } = BetState.Waiting;

	public DateTime WinlostDate { get; set; } = DateTimeOffset.UnixEpoch.Date;

	public DateTime TransactionTime { get; set; } = DateTimeOffset.UnixEpoch.Date;

	public int Settle { get; set; } = 0;

	public decimal BetAmount { get; set; } = 0;

	public decimal ActualAmount { get; set; } = 0;

	public string TxId { get; set; } = "";
}