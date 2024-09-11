using Fanex.Data.Repository;
using SabaSingleWallet.Repositories.Helpers;

namespace SabaSingleWallet.Repositories.Models.PlaceBet;

public class PlaceBetCriteria : CriteriaBase
{
	public string Action { get; set; }
	public string OperationId { get; set; }
	public string UserId { get; set; }
	public int Currency { get; set; }
	public int? MatchId { get; set; }
	public int? HomeId { get; set; }
	public int? AwayId { get; set; }
	public string? HomeName { get; set; }
	public string? AwayName { get; set; }
	public DateTime? KickOffTime { get; set; }
	public DateTime BetTime { get; set; }
	public decimal BetAmount { get; set; }
	public decimal ActualAmount { get; set; }
	public int? SportType { get; set; }
	public string? SportTypeName { get; set; }
	public int BetType { get; set; }
	public string BetTypeName { get; set; }
	public int? OddsType { get; set; }
	public long? OddsId { get; set; }
	public decimal? Odds { get; set; }
	public string? BetChoice { get; set; }
	public string? BetChoice_en { get; set; }
	public DateTime UpdateTime { get; set; }
	public DateTime? TransactionTime { get; set; }
	public int? LeagueId { get; set; }
	public string? LeagueName { get; set; }
	public string? LeagueName_en { get; set; }
	public string? SportTypeName_en { get; set; }
	public string? BetTypeName_en { get; set; }
	public string? HomeName_en { get; set; }
	public string? AwayName_en { get; set; }
	public string IP { get; set; }
	public bool? IsLive { get; set; }
	public long? TxId { get; set; }
	public string TsId { get; set; }
	public string? Point { get; set; }
	public string? Point2 { get; set; }
	public string? BetTeam { get; set; }
	public int? HomeScore { get; set; }
	public int? AwayScore { get; set; }
	public bool? BaStatus { get; set; }
	public string? Excluding { get; set; }
	public string? BetFrom { get; set; }
	public decimal CreditAmount { get; set; }
	public decimal DebitAmount { get; set; }
	public string? OddsInfo { get; set; }
	public DateTime? MatchDateTime { get; set; }
	public bool? IsOddsChanged { get; set; }
	public DateTime? WinLostDate { get; set; }
	public int? ProductId { get; set; }
	public int? GameId { get; set; }
	public string? ProductName_en { get; set; }
	public string? GameName_en { get; set; }
	public string? RoundId { get; set; }
	public string? Extra { get; set; }
	public string? ParlayType { get; set; }
	public string? Detail { get; set; }
	public string RefId { get; set; }

	public override string GetSettingKey() => StoreProcedureMapper.PlaceBetSP;
	public override bool IsValid() => true;
}