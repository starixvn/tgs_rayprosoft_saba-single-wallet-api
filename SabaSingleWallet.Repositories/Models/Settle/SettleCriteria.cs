using System.Numerics;

namespace SabaSingleWallet.Repositories.Models.Settle;

public class SettleCriteria
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public string UserId { get; set; }

	public string RefId { get; set; }

	public BigInteger TxId { get; set; }

	public DateTimeOffset UpdateTime { get; set; }

	public DateTimeOffset WinlostDate { get; set; }

	public string Status { get; set; }

	public decimal Payout { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public string? ExtraStatus { get; set; }

	public int TicketType { get; set; }
}