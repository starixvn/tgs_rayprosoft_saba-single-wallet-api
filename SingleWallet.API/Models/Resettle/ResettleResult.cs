namespace SabaSingleWallet.API.Models.Resettle;

public record ResettleResult
(
	string status,
	string? msg = null
);