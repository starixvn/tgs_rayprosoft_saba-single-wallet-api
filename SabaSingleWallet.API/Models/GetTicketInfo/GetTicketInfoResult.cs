namespace SabaSingleWallet.API.Models.GetTicketInfo;

public record GetTicketInfoResult(
	string status,
	string? tickerStatus = null, 
	decimal? actualStake = null, 
	decimal? winlostAmount = null) : ApiResult(status);

