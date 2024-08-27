
namespace SabaSingleWallet.API.Models.Placebetent;

public record PlacebetentResult
{
	public required List<PlacebetentTicketDetailResponse> TicketList { get; init; }
	public required string UserId { get; init; }
	public required decimal Balance { get; init; }
	public required string Status { get; init; } 
	public string? Msg { get; init; } 
}

public record PlacebetentTicketDetailResponse
{
	public required string RefId { get; init; } 
	public required string LicenseeTxId { get; init; } 
}