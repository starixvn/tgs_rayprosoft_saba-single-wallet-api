using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SabaSingleWallet.Repositories.Models;
[Table("settle_logs")]
public record SettleLogs
{
	[BsonId]
	public ObjectId Id { get; set; } = default!;
	/// <summary>
	/// The operator identifier
	/// </summary>
	[BsonElement("time")]
	public DateTime Time { get; set; } = default!;

	[BsonElement("request")]
	public string Request { get; set; } = default!;

	[BsonElement("response")]
	public string Response { get; set; } = default!;
}

public record SettleRequestLogs(string key, string message);
public record SettleResponseLogs(string key, string message);