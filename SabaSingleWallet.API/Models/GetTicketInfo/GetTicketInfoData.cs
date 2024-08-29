
namespace SabaSingleWallet.API.Models.GetTicketInfo;

public record GetTicketInfoData
{
	public string Action { get; set; } = "GetTicketInfo";
	public string UserId { get; set; }
	public string RefId { get; set; }
}
