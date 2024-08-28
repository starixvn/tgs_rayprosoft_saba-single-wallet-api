namespace SabaSingleWallet.API.Helpers;

public static class PlayerCodeHelpers
{
	public static string GetPlayerCode(string userId)
	{
		return userId.Split('_')[userId.Length - 2];
	}
}
