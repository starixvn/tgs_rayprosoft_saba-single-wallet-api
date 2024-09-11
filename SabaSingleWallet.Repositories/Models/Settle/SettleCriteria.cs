
using Fanex.Data.Repository;
using SabaSingleWallet.Repositories.Helpers;

namespace SabaSingleWallet.Repositories.Models.Settle;

public class SettleCriteria : CriteriaBase
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public string UserId { get; set; }

	public string RefId { get; set; }

	public long TxId { get; set; }

	public DateTimeOffset? UpdateTime { get; set; } = null;

	public DateTimeOffset? WinlostDate { get; set; } = null;

	public string Status { get; set; } = string.Empty;

	public decimal Payout { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public string? ExtraStatus { get; set; }

	public override string GetSettingKey() => StoreProcedureMapper.SettleSP;

	public override bool IsValid() => true;
}