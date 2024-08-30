using SabaSingleWallet.Core.Models.Criteria.GameBet;

namespace SabaSingleWallet.Core;

public interface IGameBetRepository
{
	Task<GameBetSetResult> GameBetSetAsync(GameBetSetCriteria criteria);
}