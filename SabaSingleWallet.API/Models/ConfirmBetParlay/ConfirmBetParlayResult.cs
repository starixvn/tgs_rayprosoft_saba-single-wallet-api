namespace SabaSingleWallet.API.Models.ConfirmBetParlay;

public record ConfirmBetParlayResult(
	string status,
	decimal balance,
	string? msg
);