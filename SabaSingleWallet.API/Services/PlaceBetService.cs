using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Core.Models.Criteria.GameBet;
using SabaSingleWallet.Core.Models.Criteria.GameRound;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetGetService(
		ICashServiceClient cashClient,
		ISapaApiMessagePublishHandler messagePublisher,
		IPlayerRepository playerRepository,
		IMapper mapper
	) : IGetService<PlaceBetRequest, PlaceBetResult>
{
	public async ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			InsertGameBet(query);
			var playerCode = query.message.UserId.ToPlayerCode();

			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>
				(
				mapper.Map<PlaceBetCriteria>(query.message),
				StoreProcedureMapper.PlaceBetSP
				);

			if (spResult.SpResult != 0)
			{
				return new PlaceBetResult("506", query.message.RefId, null, "Unable To Processed. Please Try Again Later");
			}

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(playerCode, query.message.DebitAmount).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(deductBalanceResult.ErrorMessage))
			{
				return new PlaceBetResult("506", query.message.RefId, null, "Unable To Processed. Please Try Again Later");
			}

			var res = new PlaceBetResult("0", query.message.RefId, spResult.Licenseetxid, "");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetResult("503", query.message.RefId, null, "Internal Error");
		}
	}

	private async Task InsertGameBet(PlaceBetRequest query)
	{
		var placeBetData = query.message;
		var criteria = new GameBetSetCriteria()
		{
			GameProviderId = Guid.NewGuid(), // dbo game_providers
			GameId = 123, // dbo games
			WalletId = Guid.NewGuid(), // playerInfo.walletId
			ExternalBetId = placeBetData.TsId,
			ExternalCreatedOn = DateTime.Parse(placeBetData.BetTime), // Need double check
			Id = Guid.NewGuid(), // dbo rounds
			GameProviderConfigurationId = Guid.NewGuid(), // dbo.brand_game_providers
			ResellerId = Guid.NewGuid(), //*PlayerInfo.ResellerId
			LicenseeId = Guid.NewGuid(), //*PlayerInfo.LicenseeId
			BrandId = Guid.NewGuid(), // *PlayerInfo.BrandId
			PlayerCode = placeBetData.UserId.ToPlayerCode(),
			CurrencyCode = "USD", // *PlayerInfo.CurrencyCode
			ExternalRoundId = placeBetData.TsId,
			ExternalClosedOn = null, // Formula
			ExternalPayoffOn = null,  // Formula
			ExternalResettleOn = null, //DateTimeOffset.UnixEpoch
			BetState = 0, // PlaceBet = 0
			BetType = 0, // BetType.Normal = 0
			FreeGameState = 0, //FreeGameState.None
			GambleState = 0, //GambleState.None
			CashoutState = 0, // Formular
			ResettleState = 0, //ResettleState.None
			JackpotState = 0, //JackpotState.None
			CurrencyRatio = 1.0m, // Double check
			Bet = placeBetData.DebitAmount,
			ValidBet = 0, // Formular
			Settle = 0,
			Commission = 0,
			Turnover = 0.00m, // Formular
			WinLoss = 0.00m,
			Jackpot = 0.00m,
			CreatedOn = DateTime.UtcNow,
			ClosedOn = null,
			UpdatedOn = DateTime.UtcNow,
			ResetState = 0,
			ResetOn = null,
			AdditionalData = { }, // Double check
			ActionType = placeBetData.Action // Done
		};

		return;
	}

	private async Task InsertGameRound(PlaceBetRequest query)
	{
		var placeBetData = query.message;

		var criteria = new GameRoundSetCriteria()
		{
			Id = Guid.NewGuid(),
			CreatedOn = DateTime.UtcNow,
			ExternalRoundId = "ROUND123",
			GameProviderId = Guid.NewGuid(),
			PlayerCode = "Player001",
			GameId = 123,
			CurrencyCode = "USD",
			Status = 1,
			GameProviderConfigurationId = Guid.NewGuid(),
			WalletId = Guid.NewGuid(),
			ClosedOn = null, // Nullable, can be set to null
			UpdatedOn = DateTime.UtcNow,
			BrandId = Guid.NewGuid(),
			ExternalCreatedOn = DateTime.UtcNow,
			ExternalClosedOn = null, // Nullable, can be set to null
			BetAmount = 100.00m,
			ValidBetAmount = 100.00m,
			SettleAmount = 50.00m,
			Commission = 5.00m,
			Turnover = 100.00m,
			WinLoss = 50.00m,
			TieBetAdjustment = 0.00m,
			ClientStatus = 1,
			ReopenTurnoverAdjustment = 0.00m,
			ReopenedOn = null, // Nullable, can be set to null
			EffectiveBettingPercentage = 100.0m,
			ActionType = "Settle"
		};

		return;
	}
}