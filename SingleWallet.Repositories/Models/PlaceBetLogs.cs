using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SabaSingleWallet.Repositories.Extensions.Models;
[Table("placebet_logs")]
public record PlaceBetLogs
{
	[BsonId]
	public ObjectId Id { get; set; } = default!;
	/// <summary>
	/// The operator identifier
	/// </summary>
	[BsonElement("time")]
	public DateTime Time { get; set; } = default!;

	[BsonElement("request")]
	public BsonDocument Request { get; set; } = default!;

	[BsonElement("response")]
	public BsonDocument Response { get; set; } = default!;
}

public record PlaceBetRequestLogs(string key, string message);
public record PlaceBetResponseLogs(string key, string message);