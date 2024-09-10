namespace SabaSingleWallet.API.Models.Balance;

public record GetBalanceResult
(
	string status,
	string? userId,
	decimal? balance,
	string? balanceTs
): ApiResult(status);