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

				WriteLogParams(typeof(TCriteria).Name, parameters);

				string sqlQuery = $"CALL {spName}({string.Join(", ", typeof(TCriteria).GetProperties().Select(p => $"@{p.Name.ToLower()}"))})";

				var result = await db.QueryFirstAsync<TResult>(sqlQuery, parameters);

				return result;
			}
		}

		private static void WriteLogParams(string action, DynamicParameters parameters)
		{
			var data = string.Join(", ", from param in parameters.ParameterNames select string.Format("@{0}={1}", param, parameters.Get<object>(param)));

			Console.WriteLine($"{action}: {data}");
		}
	}
}