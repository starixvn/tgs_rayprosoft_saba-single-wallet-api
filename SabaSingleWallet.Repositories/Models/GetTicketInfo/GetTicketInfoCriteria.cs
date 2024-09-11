

using Fanex.Data.Repository;
using SabaSingleWallet.Repositories.Helpers;

namespace SabaSingleWallet.Repositories.Models.GetTicketInfo;
public class GetTicketInfoCriteria : CriteriaBase
{
	public string UserId { get; set; }

	public string RefId { get; set; }

	public string? TicketStatus { get; set; } = null;

	public decimal? ActualStake { get; set; } = null;

	public decimal? WinlostAmount { get; set; } = null;

	public override string GetSettingKey() => StoreProcedureMapper.GetTicketInfo;

	public override bool IsValid() => true;

}
