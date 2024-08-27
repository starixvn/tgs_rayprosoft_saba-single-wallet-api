namespace SabaSingleWallet.API.Models.CancelBetEnt;

public record CancelBetEntRequest(
   string key,
   CancelBetEntData message
   );