namespace SabaSingleWallet.API.Models.ConfirmBet;

public record ConfirmBetRequest(
   string key,
   ConfirmBetData message
   );