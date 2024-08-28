
namespace SabaSingleWallet.Repositories.Models.CancelBet;

public class CancelBetCriteria
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public DateTime UpdateTime { get; set; }

	public string RefId { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public int SpResult { get; set; }

	public int? SportType { get; set; }
}
