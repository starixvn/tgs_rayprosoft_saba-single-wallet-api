namespace SabaSingleWallet.API.Models.PlaceBet
{
	public record PlaceBetResult(
		string? status,
		string? refId = null,
		string? licenseeTxId = null
		) : ApiResult(status);
}