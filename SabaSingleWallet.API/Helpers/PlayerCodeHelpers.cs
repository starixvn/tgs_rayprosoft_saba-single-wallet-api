namespace SabaSingleWallet.API.Helpers;

public static class PlayerCodeHelpers
{
	public static string GetPlayerCodeFromUserId(string userId)
	{
		return userId.Replace("_test", string.Empty).Remove(0, userId.IndexOf("_") + 1);
	}
}