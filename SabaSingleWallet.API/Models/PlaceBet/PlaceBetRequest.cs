using MediatR;

namespace SabaSingleWallet.API.Models.PlaceBet;

public class PlaceBetRequest : IRequest<PlaceBetResult>
{
	public string key { get; set; }

	public PlaceBetData message { get; set; }
}