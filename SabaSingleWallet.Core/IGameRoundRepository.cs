using SabaSingleWallet.Core.Models.Criteria.GameRound;

namespace SabaSingleWallet.Repositories;

public interface IGameRoundRepository
{
	Task<GameRoundSetResult> GameRoundSetAsync(GameRoundSetCriteria criteria);
}