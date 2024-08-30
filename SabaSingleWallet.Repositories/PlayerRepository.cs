using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Core;

namespace SabaSingleWallet.Repositories
{
	public class PlayerRepositoryImp : IPlayerRepository
	{
		private readonly string _connectionString;

		public PlayerRepositoryImp(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("Tgs")!;
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

		public async Task<Guid> GetProviderBycode(string code)
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = $"SELECT * FROM dbo.game_providers WHERE {code}";
				var result = await db.QueryFirstAsync<Guid>(sqlQuery);
				return result;
			}
		}
	}
}