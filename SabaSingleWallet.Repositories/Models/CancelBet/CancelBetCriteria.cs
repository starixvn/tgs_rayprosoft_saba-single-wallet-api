
using SabaSingleWallet.Repositories.Helpers;
using Fanex.Data.Repository;
namespace SabaSingleWallet.Repositories.Models.CancelBet;

public class CancelBetCriteria : CriteriaBase
{
	public string Action { get; set; }

	public string OperationId { get; set; }

	public DateTime UpdateTime { get; set; }

	public string RefId { get; set; }

	public decimal CreditAmount { get; set; }

	public decimal DebitAmount { get; set; }

	public override string GetSettingKey() => StoreProcedureMapper.CancelBetSP;

	public override bool IsValid() => true;
}
