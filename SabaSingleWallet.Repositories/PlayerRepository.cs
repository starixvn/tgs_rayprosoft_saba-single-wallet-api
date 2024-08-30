using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SabaSingleWallet.Core;
using SabaSingleWallet.Core.Models;


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

		public async Task<Object>  GetPlaceBetInfoAsync ()
		{
			var providerCode = await GetProviderByCodeAsync();
			var playserInfo = await GetPlayerInfoAsync();

			return null;
		}

		public async Task<Guid> GetProviderByCodeAsync(string code = "VR")
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sqlQuery = "SELECT ga.Id FROM dbo.game_providers ga WHERE ga.code = @Code";
				var result = await db.QueryFirstOrDefaultAsync<Guid>(sqlQuery, new { Code = code });
				return result;
			}
		}

		public async Task<object >GetPlayerInfoAsync ( string code = "4unxqhi6faxl")
		{
			using (IDbConnection db = new NpgsqlConnection(_connectionString))
			{
				string sql = @"
							SELECT
								r.Id AS ResellerId,
								r.Code AS ResellerCode,
								l.Id AS LicenseeId,
								l.Code AS LicenseeCode,
								b.Id AS BrandId,
								b.Code AS BrandCode,
								p.player_code AS PlayerCode,
								p.username AS Username,
								p.default_wallet_id AS DefaultWalletId,
								p.default_wallet_currency_code AS CurrencyCode
							FROM
								dbo.Players p
							INNER JOIN
								dbo.Brands b ON p.brand_id = b.id
							INNER JOIN
								dbo.Licensees l ON b.licensee_id = l.id
							INNER JOIN
								dbo.Resellers r ON l.reseller_id = r.id
							WHERE
								p.player_code = @PlayerCode;";

				var parameters = new { PlayerCode = code };
				var playerInfo = await db.QueryFirstOrDefaultAsync<PlayerInfo>(sql, parameters);
				return playerInfo;
			}
		}

	}
}