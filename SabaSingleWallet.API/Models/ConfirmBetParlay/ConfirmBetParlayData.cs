
namespace SabaSingleWallet.API.Models.ConfirmBetParlay;

public record ConfirmBetParlayData
{
	public required string Action { get; set; } = "confirm bet parlay";
	public required string OperationId { get; set; }
	public required string UserId { get; set; }
	public required string UpdateTime { get; set; }
	public required decimal CreditAmount { get; set; }
	public required decimal DebitAmount { get; set; }
	public required TicketInfo [] Txns { get; set; }
	public required BetParlayTicketDetail[] TicketDetail { get; set; }
	public required string TransactionTime { get; set; }
}

public record TicketInfo
{
	public required string RefId { get; set; }
	public required long TxId { get; set; }
	public required string LicenseeTxId { get; set; }
	public required decimal ActualAmount { get; set; }
	public required bool IsOddsChanged { get; set; }
	public required decimal CreditAmount { get; set; }
	public required decimal DebitAmount { get; set; }
	public required string WinlostDate { get; set; }
	public decimal? Odds { get; set; } 
}

public record BetParlayTicketDetail
{
	public required int MatchId { get; set; }
	public required int SportType { get; set; }
	public required int BetType { get; set; }
	public required int OddsId { get; set; }
	public required decimal Odds { get; set; }
	public required short OddsType { get; set; }
	public required int LeagueId { get; set; }
	public required bool IsLive { get; set; }
	public required bool IsOddsChanged { get; set; }
}