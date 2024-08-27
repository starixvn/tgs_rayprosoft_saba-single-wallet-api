namespace SabaSingleWallet.API.Models.ConfirmBetParlay;

public record ConfirmBetParlayResult(
	string statusCode,
	decimal balance,
	string? msg 
);