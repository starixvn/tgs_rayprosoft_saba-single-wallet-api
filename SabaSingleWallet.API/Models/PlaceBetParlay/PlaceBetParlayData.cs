using System.Text.Json;
using System.Text.Json.Serialization;

namespace SabaSingleWallet.API.Models.PlaceBetParlay;

public record PlaceBetParlayData
{
	public string Action { get; set; } = "PlaceBetParlay"; // default action for Bet Parlay
	public string OperationId { get; set; }
	public string UserId { get; set; }
	public int Currency { get; set; }
	public DateTime BetTime { get; set; }
	public DateTime UpdateTime { get; set; }
	public decimal TotalBetAmount { get; set; }
	public string IP { get; set; }
	public string TsId { get; set; }
	public string BetFrom { get; set; }
	public decimal CreditAmount { get; set; }
	public decimal DebitAmount { get; set; }
	public string VendorTransId { get; set; }
	public ComboInfo[] Txns { get; set; }
	public TicketDetail[] TicketDetail { get; set; }
}

//public record PlaceBetParlayData
//{
//	public required string Action { get; set; } = "PlaceBetParlay"; // default action for Bet Parlay
//	public required string OperationId { get; set; }
//	public required string UserId { get; set; }
//	public required int Currency { get; set; }
//	public required DateTime BetTime { get; set; }
//	public required DateTime UpdateTime { get; set; }
//	public required decimal TotalBetAmount { get; set; }
//	public required string IP { get; set; }
//	public string TsId { get; set; }
//	public required string BetFrom { get; set; }
//	public required decimal CreditAmount { get; set; }
//	public required decimal DebitAmount { get; set; }
//	public string VendorTransId { get; set; }
//	public required ComboInfo[] Txns { get; set; }
//	public required TicketDetail[] TicketDetail { get; set; }
//}

public record ComboInfo
{
	public required string RefId { get; set; }
	public required string ParlayType { get; set; }
	public required decimal BetAmount { get; set; }
	public required decimal CreditAmount { get; set; }
	public required decimal DebitAmount { get; set; }

	[JsonConverter(typeof(ComboInfoDetailConverter))]
	public ComboInfoDetail[] Detail { get; set; }
}

public abstract record ComboInfoDetail
{
}

public record SingleBetDetail : ComboInfoDetail
{
	public required int MatchId { get; set; }
}

public record ParlayDetail : ComboInfoDetail
{
	public required int Type { get; set; }
	public required string Name { get; set; }
	public required int BetCount { get; set; }
	public required decimal Stake { get; set; }
	public decimal? Odds { get; set; }
}

public record TicketDetail
{
	public required int MatchId { get; set; }
	public required int HomeId { get; set; }
	public required int AwayId { get; set; }
	public required string HomeName { get; set; }
	public required string AwayName { get; set; }
	public required DateTime KickOffTime { get; set; }
	public required int SportType { get; set; }
	public required string SportTypeName { get; set; }
	public required int BetType { get; set; }
	public required string BetTypeName { get; set; }
	public required int OddsId { get; set; }
	public required decimal Odds { get; set; }
	public required short OddsType { get; set; }
	public required string BetChoice { get; set; }
	public required string BetChoice_en { get; set; }
	public required int LeagueId { get; set; }
	public required string LeagueName { get; set; }
	public required bool IsLive { get; set; }
	public string Point { get; set; }
	public string Point2 { get; set; }
	public required string BetTeam { get; set; }
	public required int HomeScore { get; set; }
	public required int AwayScore { get; set; }
	public required bool BaStatus { get; set; }
	public string Excluding { get; set; }
	public required string LeagueName_en { get; set; }
	public required string SportTypeName_en { get; set; }
	public required string HomeName_en { get; set; }
	public required string AwayName_en { get; set; }
	public required string BetTypeName_en { get; set; }
	public required DateTime MatchDateTime { get; set; }
	public string BetRemark { get; set; }
}

public class ComboInfoDetailConverter : JsonConverter<ComboInfoDetail[]>
{
	public override ComboInfoDetail[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var jsonArray = JsonDocument.ParseValue(ref reader).RootElement;
		var details = new List<ComboInfoDetail>();
		foreach (var element in jsonArray.EnumerateArray())
		{
			if (element.TryGetProperty("matchId", out _))
			{
				details.Add(JsonSerializer.Deserialize<SingleBetDetail>(element.GetRawText(), options));
			}
			else if (element.TryGetProperty("type", out _))
			{
				details.Add(JsonSerializer.Deserialize<ParlayDetail>(element.GetRawText(), options));
			}
			else
			{
				throw new JsonException("Unknown detail type");
			}
		}

		return details.ToArray();
	}

	public override void Write(Utf8JsonWriter writer, ComboInfoDetail[] value, JsonSerializerOptions options)
	{ }
}