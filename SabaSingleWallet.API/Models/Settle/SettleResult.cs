namespace SabaSingleWallet.API.Models.Settle;

public record SettleResult
(
	string status
): ApiResult(status);