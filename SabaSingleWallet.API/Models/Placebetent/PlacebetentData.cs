namespace SabaSingleWallet.API.Models.Placebetent;

public record PlacebetentData
{
	public required string Action { get; init; } 
	public required string UserId { get; init; } 
	public required int Currency { get; init; } 
	public required int ProductId { get; init; }
	public required int GameId { get; init; } 
	public required List<PlaceBetentTicketDetail> TicketList { get; init; } 
	public required string IP { get; init; } 
	public required string ProductName_en { get; init; } 
	public required string GameName_en { get; init; } 
	public required string BetFrom { get; init; } 
	public required string RoundId { get; init; } 
	public required string BetTime { get; init; } 
	public required decimal CreditAmount { get; init; } 
	public required decimal DebitAmount { get; init; }
}

public record PlaceBetentTicketDetail
{
	public required string RefId { get; init; } 
	public string? BetChoiceEn { get; init; } 
	public required decimal Stake { get; init; } 
	public required decimal ActualStake { get; init; } 
	public PlaceBetentExtraInfo? Extra { get; init; }
}

public record PlaceBetentExtraInfo
{
	public int? GameNo { get; init; }
    public int? Range { get; init; }
}


