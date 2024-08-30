

namespace SabaSingleWallet.API.Models.AdjustBalance;

public record AdjustBalanceRequest(
   string key,
   AdjustBalanceData message
   );