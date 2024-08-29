using System.Data;
using System.Xml.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Repositories.Models.CancelBet;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using SabaSingleWallet.Repositories.Models.PlaceBet;

namespace SabaSingleWallet.Repositories
{
	public interface IPlayerRepository
	{
		Task<CancelBetDBResult> CallBetTransSbswCancelBetAsync(CancelBetCriteria criteria);

		Task<PlaceBetSPResult> CallBetTransSbswPlaceBetAsync(PlaceBetCriteria criteria);

		Task<ConfirmBetSPResult> CallBetTransSbswConfirmBetAsync(ConfirmBetCriteria criteria);

		Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName);
	}

	public class PlayerRepositoryImp : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs")!;
		}

		public async Task<ConfirmBetSPResult> CallBetTransSbswConfirmBetAsync(ConfirmBetCriteria criteria)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				db.Open();
				var parameters = new DynamicParameters();
				parameters.Add("action", criteria.Action);
				parameters.Add("operationid", criteria.OperationId);
				parameters.Add("updatetime", criteria.UpdateTime);
				parameters.Add("transactiontime", criteria.TransactionTime);
				parameters.Add("refid", criteria.RefId);
				parameters.Add("txid", criteria.TxId);
				parameters.Add("licenseetxid", criteria.LicenseetxId);
				parameters.Add("odds", criteria.Odds);
				parameters.Add("oddstype", criteria.OddsType);
				parameters.Add("actualamount", criteria.ActualAmount);
				parameters.Add("isoddschanged", criteria.IsOddsChanged);
				parameters.Add("creditamount", criteria.CreditAmount);
				parameters.Add("debitamount", criteria.DebitAmount);
				parameters.Add("winlostdate", criteria.WinLostDate);
				parameters.Add("spresult", criteria.SpResult);
				parameters.Add("sporttype", criteria.SportType);

				string sqlQuery = @"
					CALL dbo.bet_bettrans_sbsw_confirmbet(
						@action,
						@operationid,
						@updatetime,
						@transactiontime,
						@refid,
						@txid,
						@licenseetxid,
						@odds,
						@oddstype,
						@actualamount,
						@isoddschanged,
						@creditamount,
						@debitamount,
						@winlostdate,
						@sporttype,
						@spresult
					)";

				var result = await db.QueryFirstAsync<ConfirmBetSPResult>(sqlQuery, parameters);
				return result;
			}
		}

		public async Task<CancelBetDBResult> CallBetTransSbswCancelBetAsync(CancelBetCriteria criteria)
		{
			using IDbConnection db = new NpgsqlConnection(_connectionString);

			var parameters = new DynamicParameters();
			parameters.Add("action", criteria.Action);
			parameters.Add("operationid", criteria.OperationId);
			parameters.Add("updatetime", criteria.UpdateTime);
			parameters.Add("refid", criteria.RefId);
			parameters.Add("creditamount", criteria.CreditAmount);
			parameters.Add("debitamount", criteria.DebitAmount);
			parameters.Add("spresult", criteria.SpResult);
			parameters.Add("sporttype", criteria.SportType);

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

		public async Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				foreach (var property in typeof(TCriteria).GetProperties())
				{
					var value = property.GetValue(criteria);
					parameters.Add(property.Name.ToLower(), value);
				}

				string sqlQuery = $"CALL {spName}({string.Join(", ", typeof(TCriteria).GetProperties().Select(p => $"@{p.Name.ToLower()}"))})";

				var result = await db.QueryFirstAsync<TResult>(sqlQuery, parameters);

				return result;
			}
		}
	}
}