namespace SabaSingleWallet.API.Models.GetTicketInfo;

public record GetTicketInfoResult
{
	public string Status { get; set; }
	public string Msg { get; set; }
	public string? TicketStatus { get; set; }
	public decimal? ActualStake { get; set; }
	public decimal? WinlostAmount { get; set; }
}
