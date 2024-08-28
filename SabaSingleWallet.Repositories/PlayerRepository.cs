
using System.Data;
using System.Xml.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Repositories.Models.CancelBet;
using SabaSingleWallet.Repositories.Models.PlaceBet;


namespace SabaSingleWallet.Repositories
{
	public interface IPlayerRepository
	{
		Task <CancelBetDBResult>  CallBetTransSbswCancelBetAsync(CancelBetCriteria criteria);
		Task<PlaceBetSPResult> CallBetTransSbswPlaceBetAsync(PlaceBetCriteria criteria);
	}

	public class PlayerRepositoryImp  : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs");
		}

		public async Task<CancelBetDBResult> CallBetTransSbswCancelBetAsync(CancelBetCriteria criteria)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("action", criteria.Action);
				parameters.Add("operationid", criteria.OperationId);
				parameters.Add("updatetime", criteria.UpdateTime);
				parameters.Add("refid", criteria.RefId);
				parameters.Add("creditamount", criteria.CreditAmount);
				parameters.Add("debitamount", criteria.DebitAmount);
				parameters.Add("spresult", criteria.SpResult);
				parameters.Add("sporttype",criteria.SportType);

				string sqlQuery = @"
                    CALL dbo.bet_bettrans_sbsw_cancelbet(
                        @action,
                        @operationid,
                        @updatetime,
                        @refid,
                        @creditamount,
                        @debitamount,
                        @spresult,
                        @sporttype
                    )";

				var result = await db.QueryAsync<CancelBetDBResult>(sqlQuery, parameters);
				return result?.FirstOrDefault();
			}
		}

		public async Task<PlaceBetSPResult> CallBetTransSbswPlaceBetAsync(PlaceBetCriteria criteria)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				foreach (var property in typeof(PlaceBetCriteria).GetProperties())
				{
					var value = property.GetValue(criteria);
					parameters.Add(property.Name.ToLower(), value);
				}

				string sqlQuery = $"CALL dbo.bet_bettrans_sbsw_bet({string.Join(", ", typeof(PlaceBetCriteria).GetProperties().Select(p => $"@{p.Name.ToLower()}"))})";


				var result = await db.QueryFirstAsync<PlaceBetSPResult>(sqlQuery, parameters);

				return result;
			}
		}
	}
}