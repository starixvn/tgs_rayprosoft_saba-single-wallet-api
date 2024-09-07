namespace SabaSingleWallet.API.Models.Resettle;

public record ResettleResult
(
	string status
) : ApiResult(status);