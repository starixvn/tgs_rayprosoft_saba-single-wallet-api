namespace SabaSingleWallet.API.Extensions;

public static class UserIdConverter
{
	public static string ToPlayerCode(this string userId)
	{
		return userId.Replace("_test", string.Empty).Remove(0, userId.IndexOf("_") + 1);
	}
}