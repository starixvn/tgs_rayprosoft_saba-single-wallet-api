using System.Text.Json;
using Microsoft.Extensions.Logging;
using SabaSingleWallet.Core.Constant;
using SabaSingleWallet.Core.Models;
using SabaSingleWallet.Core.Models.Enum;
using SabaSingleWallet.Repositories;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.Core.ApplicationServices;

public class HandleBetCommandService(
	ILogger<HandleBetCommandService> logger,
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerRepository playerRepository,
	IGameBetRepository gameBetRepository,
	IGameRoundRepository roundRepository) : ICommandService<HandleBetCommand>
{
	public async ValueTask ExecuteAsync(HandleBetCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await playerRepository.GetProviderIdByCodeAsync(command.GameProviderCode);
		var playerInfo = await playerRepository.GetPlayerInfobyPlayerCodeAsync(command.PlayerCode);
		var gameProviderConfigurationId = await playerRepository.GetGameProviderConfigurationIdAsync(gameProviderId);
		var gameInfo = await playerRepository.GetGamesByProviderIdAsync(gameProviderId);

		var exchangeRates = GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode);
		var gameCurrencyInfo = GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode);

		//var id = await roundRepository.GetIdAsync(gameProviderId, gameInfo.GameId, playerInfo.WalletId, command.ExternalRoundId);
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		var roundStatus = command.BetState is BetState.Running or BetState.Running ? RoundStatus.Open : RoundStatus.Closed;
		var gameRoundId = await roundRepository.GameRoundSetAsync(new Models.Criteria.GameRound.GameRoundSetCriteria()
		{
			Id = id,
			ActionType = command.Action,
			GameProviderId = gameProviderId,
			GameProviderConfigurationId = gameProviderConfigurationId,
			GameId = gameInfo.GameId,
			BrandId = playerInfo.BrandId,
			PlayerCode = playerInfo.PlayerCode,
			CurrencyCode = playerInfo.CurrencyCode,
			WalletId = playerInfo.WalletId,
			ExternalRoundId = command.ExternalRoundId,
			ExternalCreatedOn = command.ExternalCreatedOn,
			ExternalClosedOn = command.ExternalClosedOn,
			BetAmount = ConvertPlatformRounds(exchangeRates, command.Bet),
			ValidBetAmount = ConvertPlatformRounds(exchangeRates, command.ValidBet),
			SettleAmount = ConvertPlatformRounds(exchangeRates, command.Settle),
			Commission = ConvertPlatformRounds(exchangeRates, command.Commission),
			Turnover = ConvertPlatformRounds(exchangeRates, command.Turnover),
			WinLoss = ConvertPlatformRounds(exchangeRates, command.WinLoss),
			Status = (short)roundStatus,
			CreatedOn = DateTime.Now,
			UpdatedOn = DateTime.Now,
		});
	}

	private decimal ConvertPlatformRounds(decimal exchangeRate, decimal amount)
	=> amount == 0 ? amount : decimal.Round(amount * exchangeRate, 6, MidpointRounding.ToZero);

	private decimal GetExchangeRates(string gameProviderCode, string currencyCode)
		=> GameCurrencySettings.GameProviderExchangeRates
			.Where(x => x.GameProviderCode.Equals(gameProviderCode, StringComparison.InvariantCultureIgnoreCase)
				&& x.CurrencyCode.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
			.Select(x => x.ExchangeRate)
			.SingleOrDefault(1);
}