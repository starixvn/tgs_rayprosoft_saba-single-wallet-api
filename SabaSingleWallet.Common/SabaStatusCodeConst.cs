namespace Tgs.GameRecord.Saba.Common;

public static class SabaStatusCodeConst
{
	public const string SuccessCode = "0";
	public const string ApiInternalErrorCode = "300";
	public const string NoTransactionFound = "301";

	private static readonly Dictionary<string, string> ErrorCodeDictionary = new()
	{
		{ SuccessCode, "" },
		// SP Error
		{ "101", "Duplicate Transaction" },
		{ "102", "No Data"},
		// Cash Service Error
		{ "201", "Internal Server Error" },
		{ "202", "Duplicate Transaction" },
		{ "203", "Can't found wallet" },
		{ "204", "Player Has Insufficient Funds" },
		{ "205", "Invalid Amount" },
		{ "206", "Invalid Data" },
		// Saba API Error
		{ ApiInternalErrorCode, "Internal Server Error" },
		{ NoTransactionFound, "No Transaction Found" },
	};

	public static string GetErrorMessage(string code)
	{
		return ErrorCodeDictionary.TryGetValue(code, out var errorCode)
			? errorCode
			: "Unknown Error";
	}

	public static bool IsInteruptedByCash(string code) => int.Parse(code) > 250;

	public static bool IsInteruptedBySP(string code) => int.Parse(code) > 150;

	public static bool IsSuccess(string code) => string.Equals(code, SuccessCode);
}