namespace SabaSingleWallet.Repositories.Helpers;

public static class StoreProcedureMapper
{
	public static readonly string SettleSP = "dbo.bet_bettrans_sbsw_settle"; // settle, resettle, unsettle
	public static readonly string PlaceBetSP = "dbo.bet_bettrans_sbsw_bet";
	public static readonly string ConfirmBetSP = "dbo.bet_bettrans_sbsw_confirmbet";
	public static readonly string CancelBetSP = "dbo.bet_bettrans_sbsw_cancelbet";
	public static readonly string GetTicketInfo = "dbo.bet_bettrans_sbsw_get_ticketinfo";
}