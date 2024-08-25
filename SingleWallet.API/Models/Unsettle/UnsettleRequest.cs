namespace SabaSingleWallet.API.Models.Unsettle;

public record UnsettleRequest(
	   string key,
	   UnsettleData message
	   );