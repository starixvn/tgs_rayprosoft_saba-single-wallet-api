namespace SabaSingleWallet.API.Extensions;

public static class UserIdConverter
{
	public static string ToPlayerCode(this string userId)
	{
		//TODO recheck with userId without _test posfix
		return userId.Split('_')[userId.Length - 2];
	}
}