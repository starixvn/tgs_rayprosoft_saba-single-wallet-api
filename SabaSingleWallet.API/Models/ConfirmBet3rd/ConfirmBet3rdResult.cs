namespace SabaSingleWallet.API.Models.ConfirmBet3rd;
public record ConfirmBet3rdResult(
	string Status, 
	string? Msg, 
	decimal Balance 
);