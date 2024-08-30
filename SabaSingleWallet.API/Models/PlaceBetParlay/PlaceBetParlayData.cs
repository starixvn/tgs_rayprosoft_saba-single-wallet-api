using System.Text.Json;
using System.Text.Json.Serialization;

namespace SabaSingleWallet.API.Models.PlaceBetParlay;

public record PlaceBetParlayData
{
	public string Action { get; set; } = "PlaceBetParlay"; // default action for Bet Parlay
	public string OperationId { get; set; }
	public string UserId { get; set; }
	public int Currency { get; set; }
	public string BetTime { get; set; }
	public string UpdateTime { get; set; }
	public decimal TotalBetAmount { get; set; }
	public string IP { get; set; }
	public string? TsId { get; set; }
	public string BetFrom { get; set; }
	public decimal CreditAmount { get; set; }
	public decimal DebitAmount { get; set; }
	public string? VendorTransId { get; set; }
	public ComboInfo[] Txns { get; set; }
	public TicketDetail[] TicketDetail { get; set; }
}

public record ComboInfo
{
	public string RefId { get; set; }
	public string ParlayType { get; set; }
	public decimal BetAmount { get; set; }
	public decimal CreditAmount { get; set; }
	public decimal DebitAmount { get; set; }

	[JsonConverter(typeof(ComboInfoDetailConverter))]
	public ComboInfoDetail[] Detail { get; set; }
}

public abstract record ComboInfoDetail
{
}

public record SingleBetDetail : ComboInfoDetail
{
	public int MatchId { get; set; }
}

public record ParlayDetail : ComboInfoDetail
{
	public int Type { get; set; }
	public string Name { get; set; }
	public int BetCount { get; set; }
	public decimal Stake { get; set; }
	public decimal? Odds { get; set; }
}

public record TicketDetail
{
	public int MatchId { get; set; }
	public int HomeId { get; set; }
	public int AwayId { get; set; }
	public string HomeName { get; set; }
	public string AwayName { get; set; }
	public string KickOffTime { get; set; }
	public int SportType { get; set; }
	public string SportTypeName { get; set; }
	public int BetType { get; set; }
	public string BetTypeName { get; set; }
	public int OddsId { get; set; }
	public decimal Odds { get; set; }
	public short OddsType { get; set; }
	public string BetChoice { get; set; }
	public string BetChoice_en { get; set; }
	public int LeagueId { get; set; }
	public string LeagueName { get; set; }
	public bool IsLive { get; set; }
	public string? Point { get; set; }
	public string? Point2 { get; set; }
	public string BetTeam { get; set; }
	public int HomeScore { get; set; }
	public int AwayScore { get; set; }
	public bool BaStatus { get; set; }
	public string? Excluding { get; set; }
	public string LeagueName_en { get; set; }
	public string SportTypeName_en { get; set; }
	public string HomeName_en { get; set; }
	public string AwayName_en { get; set; }
	public string BetTypeName_en { get; set; }
	public string MatchDateTime { get; set; }
	public string? BetRemark { get; set; }
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