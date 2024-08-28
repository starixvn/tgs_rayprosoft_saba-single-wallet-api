namespace SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;

public record BetTransSBSWConfirmBetCriteria
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public DateTime UpdateTime { get; set; }

	public DateTime TransactionTime { get; set; }

	public string RefId { get; set; }

	public long TxId { get; set; }

	public long LicenseetxId { get; set; }

	public decimal Odds { get; set; }

	public int OddsType { get; set; }

	public decimal ActualAmount { get; set; }

	public bool IsOddsChanged { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public DateTime WinLostDate { get; set; }

	public int? SportType { get; set; }

	public int? SpResult { get; set; }
}