using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Repositories.Models.GameRound;

namespace SabaSingleWallet.Repositories;

public class GameRoundRepository : IGameRoundRepository
{
	private readonly string _connectionString;

	public GameRoundRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("Tgs")!;
	}

	public async Task<GameRoundSetResult> GameRoundSetAsync(GameRoundSetCriteria criteria)
	{
		using (IDbConnection db = new NpgsqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();

			parameters.Add("@id", criteria.Id, DbType.Guid);
			parameters.Add("@created_on", criteria.CreatedOn, DbType.DateTime);
			parameters.Add("@external_round_id", criteria.ExternalRoundId, DbType.String);
			parameters.Add("@game_provider_id", criteria.GameProviderId, DbType.Guid);
			parameters.Add("@player_code", criteria.PlayerCode, DbType.String);
			parameters.Add("@game_id", criteria.GameId, DbType.Int32);
			parameters.Add("@currency_code", criteria.CurrencyCode, DbType.String);
			parameters.Add("@status", criteria.Status, DbType.Int32);
			parameters.Add("@game_provider_configuration_id", criteria.GameProviderConfigurationId, DbType.Guid);
			parameters.Add("@wallet_id", criteria.WalletId, DbType.Guid);
			parameters.Add("@closed_on", criteria.ClosedOn, DbType.DateTime);
			parameters.Add("@updated_on", criteria.UpdatedOn, DbType.DateTime);
			parameters.Add("@brand_id", criteria.BrandId, DbType.Guid);
			parameters.Add("@external_created_on", criteria.ExternalCreatedOn, DbType.DateTime);
			parameters.Add("@external_closed_on", criteria.ExternalClosedOn, DbType.DateTime);
			parameters.Add("@bet_amount", criteria.BetAmount, DbType.Decimal);
			parameters.Add("@valid_bet_amount", criteria.ValidBetAmount, DbType.Decimal);
			parameters.Add("@settle_amount", criteria.SettleAmount, DbType.Decimal);
			parameters.Add("@commission", criteria.Commission, DbType.Decimal);
			parameters.Add("@turnover", criteria.Turnover, DbType.Decimal);
			parameters.Add("@win_loss", criteria.WinLoss, DbType.Decimal);
			parameters.Add("@tie_bet_adjustment", criteria.TieBetAdjustment, DbType.Decimal);
			parameters.Add("@client_status", criteria.ClientStatus, DbType.Int32);
			parameters.Add("@reopen_turnover_adjustment", criteria.ReopenTurnoverAdjustment, DbType.Decimal);
			parameters.Add("@reopened_on", criteria.ReopenedOn, DbType.DateTime);
			parameters.Add("@effective_betting_percentage", criteria.EffectiveBettingPercentage, DbType.Decimal);
			parameters.Add("@actiontype", criteria.ActionType, DbType.String);

			string sqlQuery = "CALL dbo.bet_rounds_set(" +
						"id => @id, " +
						"created_on => @created_on, " +
						"external_round_id => @external_round_id, " +
						"game_provider_id => @game_provider_id, " +
						"player_code => @player_code, " +
						"game_id => @game_id, " +
						"currency_code => @currency_code, " +
						"status => @status, " +
						"game_provider_configuration_id => @game_provider_configuration_id, " +
						"wallet_id => @wallet_id, " +
						"closed_on => @closed_on, " +
						"updated_on => @updated_on, " +
						"brand_id => @brand_id, " +
						"external_created_on => @external_created_on, " +
						"external_closed_on => @external_closed_on, " +
						"bet_amount => @bet_amount, " +
						"valid_bet_amount => @valid_bet_amount, " +
						"settle_amount => @settle_amount, " +
						"commission => @commission, " +
						"turnover => @turnover, " +
						"win_loss => @win_loss, " +
						"tie_bet_adjustment => @tie_bet_adjustment, " +
						"client_status => @client_status, " +
						"reopen_turnover_adjustment => @reopen_turnover_adjustment, " +
						"reopened_on => @reopened_on, " +
						"effective_betting_percentage => @effective_betting_percentage, " +
						"actiontype => @actiontype);";

			var result = await db.QueryFirstAsync<GameRoundSetResult>(sqlQuery, parameters);

			return result;
		}
	}
}