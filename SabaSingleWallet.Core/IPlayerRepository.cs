using SabaSingleWallet.Core.Models;

namespace SabaSingleWallet.Core;

public interface IPlayerRepository
{
	Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName);

	Task<Guid> GetProviderIdByCodeAsync(string code = "VR");

	Task<PlayerInfo> GetPlayerInfobyPlayerCodeAsync(string code = "4unxqhi6faxl");

	Task<GameInfo> GetGamesByProviderIdAsync(Guid gameProviderId = default);

	Task<Guid> GetGameProviderConfigurationIdAsync(Guid gameProviderId = default,
			Guid brandId = default);

	Task<Object> GetPlaceBetInfoAsync();
}