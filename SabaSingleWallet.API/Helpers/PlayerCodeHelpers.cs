namespace SabaSingleWallet.API.Helpers;

public static class PlayerCodeHelpers
{
	public static string GetPlayerCode(string userId)
	{
		return userId.Split('_')[userId.Length - 2];
	}

	public static string GetPlayerCodeFromUserId(string userId)
	{
		return userId.Replace("_test", string.Empty).Remove(0, userId.IndexOf("_") + 1);
	}
}