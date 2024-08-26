namespace SabaSingleWallet.API.Models.PlaceBetParlay;

public record PlaceBetParlayResult
(	string status,
	TicketInfoMapping[]? txns,
	string? msg);

public record TicketInfoMapping(
	string? refId,
	string? licenseeTxId
);