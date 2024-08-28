
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace SabaSingleWallet.Repositories
{
	public interface IPlayerRepository
	{
		Task<object> GetTableNames();

		Task<object> CallBetTransSbswCancelBetAsync();
	}

	public class PlayerRepositoryImp  : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs");
		}

		public async Task<object> GetTableNames()
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = "SELECT * FROM information_schema.tables";
				var result = await db.QueryAsync<object>(sqlQuery);
				return result;
			}
		}

		public async Task<object> CallBetTransSbswCancelBetAsync()
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("action", "action1", DbType.String);
				parameters.Add("operationid", "opid1", DbType.String);
				parameters.Add("updatetime", new DateTime(2024, 8, 27, 5, 51, 56, 573, DateTimeKind.Utc), DbType.DateTime);
				parameters.Add("refid", "refid1", DbType.String);
				parameters.Add("creditamount", 11.1m, DbType.Decimal);
				parameters.Add("debitamount", 1.1m, DbType.Decimal);
				parameters.Add("spresult", 0, DbType.Int32);
				parameters.Add("sporttype", null, DbType.Int32);

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

				var result = await db.QueryAsync<object>(sqlQuery, parameters);
				return result;
			}
		}
	}
}