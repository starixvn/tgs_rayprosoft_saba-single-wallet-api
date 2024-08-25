using System.Text.Json;
using MongoDB.Driver;
using SabaSingleWallet.Repositories.Extensions.Models;

namespace SabaSingleWallet.Repositories;

public interface IConfirmBetLogsRepository
{
	ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default);
}

internal class ConfirmBetLogsRepository(IMongoClient client) : IConfirmBetLogsRepository
{
	public async ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var document = new ConfirmBetLogs()
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

	private IMongoCollection<ConfirmBetLogs> GetCollection()
	=> client
	.GetDatabase($"sw_logs_master")
	.GetCollection<ConfirmBetLogs>("place_bet_logs");
}