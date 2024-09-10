namespace SabaSingleWallet.API.Models.Unsettle;

public record UnsettleResult
(
	string status
) : ApiResult(status);