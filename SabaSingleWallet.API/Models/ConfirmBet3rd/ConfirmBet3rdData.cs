namespace SabaSingleWallet.API.Models.ConfirmBet3rd;

public record  ConfirmBet3rdData
{
	public required string Action { get; init; } = "confirm bet 3rd";
	public required string UserId { get; init; } 
	public required string OperationId { get; init; } 
	public required string UpdateTime { get; init; } 
	public required List<Bet3rdTicketDetail> Txns { get; init; } 
	public required string TransactionTime { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; } 
}

public record Bet3rdTicketDetail
{
	public required string RefId { get; init; }
	public required long TxId { get; init; } 
	public required string LicenseeTxId { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; } 
	public required string WinlostDate { get; init; }
}