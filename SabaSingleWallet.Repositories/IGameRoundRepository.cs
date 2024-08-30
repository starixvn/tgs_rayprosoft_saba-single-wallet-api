using SabaSingleWallet.Repositories.Models.GameRound;

namespace SabaSingleWallet.Repositories;

public interface IGameRoundRepository
{
	Task<GameRoundSetResult> GameRoundSetAsync(GameRoundSetCriteria criteria);
}