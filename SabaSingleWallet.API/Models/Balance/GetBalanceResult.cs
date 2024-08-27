namespace SabaSingleWallet.API.Models.Balance;

public record GetBalanceResult
(
	string status,
	string? userId,
	decimal? balance,
	DateTime? balanceTs,
	string? msg = null
);