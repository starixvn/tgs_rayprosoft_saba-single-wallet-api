namespace SabaSingleWallet.API.Models.CancelBet;

public record CancelBetResult
(
	string status,
	decimal? balance,
	string? msg = null
);