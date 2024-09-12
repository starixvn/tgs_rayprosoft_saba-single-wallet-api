using Tgs.CashService;

namespace SabaSingleWallet.Common;

public static class ProviderConst
{
	public static readonly string ProviderCode = "SABASW";
	public static readonly string GameExternalId = "sport_book";

	public static ProviderInfo CreateProviderInfo(string refId, string operationId, string action)
		=> new(ProviderCode, refId, operationId, action);
}