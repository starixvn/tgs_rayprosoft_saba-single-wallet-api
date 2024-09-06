using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Models;

public record ApiResult(string? status)
{
	public string msg => SabaStatusCodeConst.GetErrorMessage(status ?? "0");
}