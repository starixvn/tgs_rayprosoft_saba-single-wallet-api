namespace SabaSingleWallet.Repositories.Models.Resettle;

public class ResettleCriteria
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public string UserId { get; set; }

	public string RefId { get; set; }

	public long TxId { get; set; }

	public DateTimeOffset UpdateTime { get; set; }

	public DateTimeOffset WinlostDate { get; set; }

	public string Status { get; set; }

	public decimal Payout { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public string? ExtraStatus { get; set; }
}