using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Resettle;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ResettleService(
	ISapaApiMessagePublishHandler messagePublisher,
	ICashServiceClient cashServiceClient,
	IPlayerRepository playerRepository,
	IMapper mapper,
	ISentryClient sentryClient
	) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		ResettleCriteria? criteria = null;

		try
		{
			foreach (var settlement in query.message.Txns)
			{
				criteria = mapper.Map<ResettleCriteria>(settlement);
				var spResult = await playerRepository.CallStoredProcedureAsync<ResettleCriteria, ResettleSPResult>
					(criteria, StoreProcedureMapper.SettleSP);

				//TODO skip if duplicated transaction
				if (SabaStatusCodeConst.IsInteruptedBySP(spResult.StatusCode))
				{
					return await GenerateResultAsync(query, spResult.StatusCode);
				}

				if (!settlement.ExtraInfo.IsOnlyWinlostDateChanged)
				{
					var playerCode = settlement.UserId.ToPlayerCode();

					var balanceResponse = await cashServiceClient.GameProviderAdjustBalanceAsync(
						playerCode,
						settlement.CreditAmount,
						settlement.DebitAmount,
						new ProviderInfo(
							ProviderConst.ProviderCode,
							settlement.RefId,
							query.message.OperationId,
							query.message.Action),
						cancellationToken: cancellationToken);

					//TODO skip if duplicated transaction
					//TODO skip if player has inssufficient amount (half lose -> lose, void)

					if (SabaStatusCodeConst.IsInteruptedByCash(balanceResponse.ErrorCode))
					{
						return await GenerateResultAsync(query, balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Resettle(criteria, settlement.ExtraInfo.IsOnlyWinlostDateChanged), cancellationToken);
			}

			return await GenerateResultAsync(query, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			if (criteria != null)
			{
				sentryClient.CaptureMessage(message: "EXEC " + criteria?.ToString(), SentryLevel.Error);
			}

			sentryClient.CaptureException(ex);

			return await GenerateResultAsync(query, SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private async Task<ResettleResult> GenerateResultAsync(ResettleRequest query, string status)
	{
		var res = new ResettleResult(status);
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = query.key,
			Action = query.message.Action,
			Request = query,
			Response = res
		});

		return res;
	}
}