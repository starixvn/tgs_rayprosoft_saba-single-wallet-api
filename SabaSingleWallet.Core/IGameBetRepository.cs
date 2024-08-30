using SabaSingleWallet.Core.Models.Criteria.GameBet;

namespace SabaSingleWallet.Core;

public interface IGameBetRepository
{
	Task GameBetSetAsync(GameBetSetCriteria criteria);
}