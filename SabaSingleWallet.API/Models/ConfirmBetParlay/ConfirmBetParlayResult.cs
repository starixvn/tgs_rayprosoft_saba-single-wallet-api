namespace SabaSingleWallet.API.Models.ConfirmBetParlay;

public record ConfirmBetParlayResult(decimal balance, string status = "0") : ApiResult(status);