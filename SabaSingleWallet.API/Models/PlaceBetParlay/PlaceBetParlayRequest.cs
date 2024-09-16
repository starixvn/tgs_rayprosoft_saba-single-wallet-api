using MediatR;

namespace SabaSingleWallet.API.Models.PlaceBetParlay;
public record PlaceBetParlayRequest(
	string key,
	PlaceBetParlayData message
) : IRequest<PlaceBetParlayResult>;