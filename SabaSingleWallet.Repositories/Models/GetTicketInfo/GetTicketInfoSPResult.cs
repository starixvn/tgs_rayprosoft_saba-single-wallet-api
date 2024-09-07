namespace SabaSingleWallet.Repositories.Models.GetTicketInfo;

public class GetTicketInfoSPResult : SPResult
{
	public string SpResult { get; set; }
	public string? TicketStatus { get; set; }
	public decimal? ActualStake { get; set; }
	public decimal? WinlostAmount { get; set; }
}