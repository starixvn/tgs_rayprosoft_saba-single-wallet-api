namespace SabaSingleWallet.API.Models.SettleEnt;

public record SettleEntResult
(
	string status,
	string? msg = null
);