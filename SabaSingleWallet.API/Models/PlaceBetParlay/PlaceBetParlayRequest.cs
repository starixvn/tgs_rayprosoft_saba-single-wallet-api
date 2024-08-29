using System.Dynamic;

namespace SabaSingleWallet.API.Models.PlaceBetParlay;
public record PlaceBetParlayRequest(
	string key,
	DynamicObject message
);