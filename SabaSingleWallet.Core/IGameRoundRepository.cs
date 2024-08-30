using SabaSingleWallet.Core.Models.Criteria.GameRound;

namespace SabaSingleWallet.Repositories;

public interface IGameRoundRepository
{
	Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId);

	Task<GameRoundSetResult> GameRoundSetAsync(GameRoundSetCriteria criteria);
}