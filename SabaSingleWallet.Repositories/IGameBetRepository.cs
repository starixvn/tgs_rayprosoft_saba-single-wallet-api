using SabaSingleWallet.Repositories.Models.GameBet;

namespace SabaSingleWallet.Repositories;

public interface IGameBetRepository
{
	Task<GameBetSetResult> GameBetSetAsync(GameBetSetCriteria criteria);
}