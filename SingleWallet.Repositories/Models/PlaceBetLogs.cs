using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SabaSingleWallet.Repositories.Extensions.Models;
[Table("place_bet_logs")]
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
	public Object Request { get; set; } = default!;

	[BsonElement("response")]
	public Object Response { get; set; } = default!;
}
