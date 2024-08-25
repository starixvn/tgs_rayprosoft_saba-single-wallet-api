namespace SabaSingleWallet.API.Models.PlaceBet;

public record PlaceBetRequest(
   string key,
   PlaceBetData message
   );