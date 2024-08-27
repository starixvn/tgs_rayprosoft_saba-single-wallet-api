
namespace SabaSingleWallet.API.Models.Placebet3rd;

public record Placebet3rdResult
{
	public required List<Placebet3rdTicketInfoMapping> Txns { get; init; } 
	public required decimal Balance { get; init; } 
	public required string Status { get; init; } 
	public string? Msg { get; init; } 
}

public record Placebet3rdTicketInfoMapping
{
	public required string RefId { get; init; }
	public required string LicenseeTxId { get; init; } 
}