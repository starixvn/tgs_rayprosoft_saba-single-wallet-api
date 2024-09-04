using System.Text.Json;
using Microsoft.Extensions.Logging;
using SabaSingleWallet.Core.Constant;
using SabaSingleWallet.Core.Models.Criteria.GameBet;
using SabaSingleWallet.Core.Models.Enum;
using SabaSingleWallet.Repositories;
using Sentry;
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
		try
		{
			var gameProviderId = await playerRepository.GetProviderIdByCodeAsync(command.GameProviderCode);
			var playerInfo = await playerRepository.GetPlayerInfobyPlayerCodeAsync(command.PlayerCode);
			var gameProviderConfigurationId = await playerRepository.GetGameProviderConfigurationIdAsync(gameProviderId, playerInfo.BrandId);
			var gameInfo = await playerRepository.GetGamesByProviderIdAsync(gameProviderId);

			var exchangeRates = GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode);
			var gameCurrencyInfo = GetExchangeRates(playerInfo.PlayerCode, gameInfo.GameProviderCode);

			var existedRoundId = await roundRepository.GetIdAsync(gameProviderId, gameInfo.GameId, playerInfo.WalletId, command.ExternalRoundId);
			var id = existedRoundId.HasValue ? existedRoundId : await serialNumberGenerator.GenerateAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			var roundStatus = command.BetState is BetState.Waiting or BetState.Running ? RoundStatus.Open : RoundStatus.Closed;
			var roundCriteria = new Models.Criteria.GameRound.GameRoundSetCriteria()
			{
				Id = (Guid)id,
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
				BetType = (short)command.BetType,
				BetAmount = ConvertPlatformRounds(exchangeRates, command.Bet),
				ActualAmount = ConvertPlatformRounds(exchangeRates, command.ActualAmount),
				ValidBetAmount = ConvertPlatformRounds(exchangeRates, command.ValidBet),
				SettleAmount = ConvertPlatformRounds(exchangeRates, command.Settle),
				Commission = ConvertPlatformRounds(exchangeRates, command.Commission),
				Turnover = ConvertPlatformRounds(exchangeRates, command.Turnover),
				Payout = ConvertPlatformRounds(exchangeRates, command.Payout),
				Status = (short)roundStatus,
				CreatedOn = DateTime.Now,
				UpdatedOn = DateTime.Now,
			};
			await roundRepository.GameRoundSetAsync(roundCriteria);

			var gameBetCriteria = new GameBetSetCriteria()
			{
				GameProviderId = gameProviderId,
				GameId = gameInfo.GameId,
				WalletId = playerInfo.WalletId,
				ExternalBetId = command.ExternalBetId,
				ExternalCreatedOn = command.ExternalCreatedOn.ToUniversalTime(),
				Id = (Guid)id,
				GameProviderConfigurationId = gameProviderConfigurationId,
				ResellerId = playerInfo.ResellerId,
				LicenseeId = playerInfo.LicenseeId,
				BrandId = playerInfo.BrandId,
				PlayerCode = playerInfo.PlayerCode,
				CurrencyCode = playerInfo.CurrencyCode,
				ExternalRoundId = command.ExternalRoundId,
				ExternalClosedOn = command.ExternalClosedOn.ToUniversalTime(),
				ExternalPayoffOn = command.ExternalPayoffOn.ToUniversalTime(),
				ExternalResettleOn = command.ExternalResettleOn.ToUniversalTime(),
				BetState = (int)command.BetState,
				BetType = (int)command.BetType,
				FreeGameState = (int)command.FreeGameState,
				GambleState = (int)command.GambleState,
				CashoutState = (int)command.CashoutState,
				ResettleState = (int)command.ResettleState,
				JackpotState = (int)command.JackpotState,
				CurrencyRatio = exchangeRates, // Double check
				Bet = ConvertPlatformRounds(exchangeRates, command.Bet),
				ActualAmount = ConvertPlatformRounds(exchangeRates, command.ActualAmount),
				ValidBet = ConvertPlatformRounds(exchangeRates, command.ValidBet),
				Settle = ConvertPlatformRounds(exchangeRates, command.Settle),
				Commission = ConvertPlatformRounds(exchangeRates, command.Commission),
				Turnover = ConvertPlatformRounds(exchangeRates, command.Turnover),
				Payout = ConvertPlatformRounds(exchangeRates, command.Payout),
				Jackpot = ConvertPlatformRounds(exchangeRates, command.Jackpot),
				ResetState = 0,
				CreatedOn = DateTimeOffset.UtcNow,
				ClosedOn = DateTimeOffset.UnixEpoch.UtcDateTime,
				UpdatedOn = DateTimeOffset.UtcNow,
				ResetOn = DateTimeOffset.UnixEpoch.UtcDateTime,
				ActionType = command.Action
			};

			await gameBetRepository.GameBetSetAsync(gameBetCriteria);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
			SentrySdk.CaptureException(ex);
		}
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