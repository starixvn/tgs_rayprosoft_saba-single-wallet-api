namespace SabaSingleWallet.API.Models.Resettle;

public record ResettleRequest(
   string key,
   ResettleData message
   );