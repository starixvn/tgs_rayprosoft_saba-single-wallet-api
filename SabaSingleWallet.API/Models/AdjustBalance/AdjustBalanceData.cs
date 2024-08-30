namespace SabaSingleWallet.API.Models.AdjustBalance;

public record AdjustBalanceData
{
	public required string  Action { get; set; } 
	public required string Time { get; set; } 
	public required string UserId { get; set; } 
	public required int Currency { get; set; } 
	public required long TxId { get; set; }
	public long? RefNo { get; set; }
	public required string RefId { get; set; } 
	public required string OperationId { get; set; }
	public required int BetType { get; set; } 
	public required string BetTypeName { get; set; }
	public required string WinlostDate { get; set; }
	public required BalanceInfo BalanceInfo { get; set; } 
}

public record BalanceInfo
{
	public required decimal CreditAmount { get; set; }
	public required decimal DebitAmount { get; set; }
}