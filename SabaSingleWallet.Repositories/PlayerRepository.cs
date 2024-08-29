using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Repositories.Models.CancelBet;

namespace SabaSingleWallet.Repositories
{
	public interface IPlayerRepository
	{
		Task<CancelBetDBResult> CallBetTransSbswCancelBetAsync(CancelBetCriteria criteria);

		Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName);
	}

	public class PlayerRepositoryImp : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs")!;
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