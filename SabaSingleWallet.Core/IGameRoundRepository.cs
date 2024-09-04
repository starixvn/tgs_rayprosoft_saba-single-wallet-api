using SabaSingleWallet.Core.Models.Criteria.GameRound;

namespace SabaSingleWallet.Repositories;

public interface IGameRoundRepository
{
	Task<Guid?> GetIdAsync(Guid gameProviderId, int gameId, Guid walletId, string externalRoundId);

	Task GameRoundSetAsync(GameRoundSetCriteria criteria);

	Task<decimal> GameRoundDetailSetAsync(GameRoundDetailSetCriteria criteria);
}