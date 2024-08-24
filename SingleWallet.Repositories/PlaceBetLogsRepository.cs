using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
	public async ValueTask<bool> AddAsync(Object req, Object res, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var document = new PlaceBetLogs()
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

	private IMongoCollection<PlaceBetLogs> GetCollection()
	=> client
	.GetDatabase($"sw_logs_master")
	.GetCollection<PlaceBetLogs>("place_bet_logs");
}