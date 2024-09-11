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
				var properties = new List<string>();

				foreach (var property in typeof(TCriteria).GetProperties())
				{
					var value = property.GetValue(criteria);

					if (value is not null)
					{
						parameters.Add(property.Name, value);
						properties.Add(property.Name);
					}
				}

				string sqlQuery = $"CALL {spName}({string.Join(", ", properties.Select(property => $"{property} => @{property}"))})";

				var result = await db.QueryFirstAsync<TResult>(sqlQuery, parameters);

				return result;
			}
		}
	}
}