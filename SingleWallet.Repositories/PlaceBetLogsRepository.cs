using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using SabaSingleWallet.Repositories.Extensions.Models;

namespace SabaSingleWallet.Repositories;

public interface IPlaceBetLogsRepository
{
	ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default);
}

internal class PlaceBetLogsRepository(IMongoClient client) : IPlaceBetLogsRepository
{
	private static readonly JsonSerializerOptions _WriteOptions = new()
	{
		WriteIndented = true
	};

	public async ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var document = new PlaceBetLogs()
		{
			Time = DateTime.UtcNow,
			Request = BsonDocument.Parse(JsonSerializer.Serialize(req, _WriteOptions)),
			Response = BsonDocument.Parse(JsonSerializer.Serialize(res, _WriteOptions))
		};

		await collection.InsertOneAsync(
			document: document,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return true;
	}

	private IMongoCollection<PlaceBetLogs> GetCollection()
	=> client
	.GetDatabase($"sw_logs_master")
	.GetCollection<PlaceBetLogs>("placebet_logs");
}