using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SabaSingleWallet.Repositories.Models;
[Table("getbalance_logs")]
public record GetBalanceLogs
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

public record GetBalanceRequestLogs(string key, string message);
public record GetBalanceResponseLogs(string key, string message);