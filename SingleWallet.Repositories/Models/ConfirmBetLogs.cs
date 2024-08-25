using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SabaSingleWallet.Repositories.Extensions.Models;
[Table("confirmbet_logs")]
public record ConfirmBetLogs
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

public record ConfirmBetRequestLogs(string key, string message);
public record ConfirmBetResponseLogs(string key, string message);