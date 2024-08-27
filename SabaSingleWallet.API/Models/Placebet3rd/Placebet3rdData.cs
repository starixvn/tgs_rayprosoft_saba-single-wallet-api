
namespace SabaSingleWallet.API.Models.Placebet3rd;

public record Placebet3rdData
{
	public required string Action { get; init; } 
	public required string OperationId { get; init; } 
	public required string UserId { get; init; } 
	public required int Currency { get; init; } 
	public required int ProductId { get; init; } 
	public required int GameId { get; init; } 
	public required Placebet3rdTicketDetail [] TicketList { get; init; } 
	public required string BetTime { get; init; } 
	public required string IP { get; init; } 
	public string? TsId { get; init; } 
	public required string ProductName_en { get; init; } 
	public required string GameName_en { get; init; }
	public required string BetFrom { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; } 
}
public record Placebet3rdTicketDetail
{
	public required string RefId { get; init; } 
	public required string BetChoice_en { get; init; } 
	public required decimal Odds { get; init; }
	public required short OddsType { get; init; } 
	public required decimal BetAmount { get; init; } 
	public required decimal ActualAmount { get; init; } 
	public object? Extra { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; }
}

public abstract record Placebet3rdExtraInfo  // extra ticketdetail 
{
	public required int GameNo { get; init; }
	public required string GameBetType { get; init; }
}


public record ExtraInfoRNGKeno : Placebet3rdExtraInfo
{
	public required int Range { get; init; } 
}

public record ExtraInfoLottery : Placebet3rdExtraInfo
{
}

public record ExtraInfoTableGame : Placebet3rdExtraInfo
{
}