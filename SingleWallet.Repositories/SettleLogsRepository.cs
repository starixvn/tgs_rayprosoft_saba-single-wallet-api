using System.Text.Json;
using MongoDB.Driver;
using SabaSingleWallet.Repositories.Models;

namespace SabaSingleWallet.Repositories;

public interface ISettleLogsRepository
{
	ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default);
}

internal class SettleLogsRepository(IMongoClient client) : ISettleLogsRepository
{
	public async ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var document = new SettleLogs()
		{
			Time = DateTime.UtcNow,
			Request = JsonSerializer.Serialize(req, new JsonSerializerOptions { WriteIndented = true }),
			Response = JsonSerializer.Serialize(res, new JsonSerializerOptions { WriteIndented = true })
		};

		await collection.InsertOneAsync(
			document: document,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return true;
	}

	private IMongoCollection<SettleLogs> GetCollection()
	=> client
	.GetDatabase($"sw_logs_master")
	.GetCollection<SettleLogs>("settle_logs");
}