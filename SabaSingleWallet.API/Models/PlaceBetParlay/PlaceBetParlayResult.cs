using System.Diagnostics.CodeAnalysis;

namespace SabaSingleWallet.API.Models.PlaceBetParlay;

public record PlaceBetParlayResult(List<TicketInfoMapping>? txns, string status = "0") : ApiResult(status);

public record TicketInfoMapping(string? refId, string? licenseeTxId);