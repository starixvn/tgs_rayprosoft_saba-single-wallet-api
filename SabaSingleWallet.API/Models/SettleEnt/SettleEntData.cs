namespace SabaSingleWallet.API.Models.SettleEnt;

public class SettleEntData
{
	public required string Action { get; init; } 
	public required string UserId { get; init; } 
	public required string RefId { get; init; } 
	public required string Status { get; init; } 
	public required decimal ActualStake { get; init; } 
	public required decimal Stake { get; init; } 
	public required decimal NetStake { get; init; } 
	public required string WinlostDate { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; } 
	public required decimal WinlostAmount { get; init; } 
	public required List<long> TxIds { get; init; } 
}
