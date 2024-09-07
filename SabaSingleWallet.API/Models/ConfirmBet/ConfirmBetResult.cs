namespace SabaSingleWallet.API.Models.ConfirmBet;

public record ConfirmBetResult(
		string status,
		decimal? balance = null
		) : ApiResult(status);