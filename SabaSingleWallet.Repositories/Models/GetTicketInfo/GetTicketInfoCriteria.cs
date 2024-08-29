

namespace SabaSingleWallet.Repositories.Models.GetTicketInfo;
public class GetTicketInfoCriteria
{
	public string UserId { get; set; }

	public string RefId { get; set; }

	public string? TicketStatus { get; set; } = null;

	public decimal? ActualStake { get; set; } = null;

	public decimal? WinlostAmount { get; set; } = null;

}
