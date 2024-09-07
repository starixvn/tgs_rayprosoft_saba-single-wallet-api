using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ResettleService(
	ISapaApiMessagePublishHandler messagePublisher,
	ICashServiceClient cashServiceClient,
	IPlayerRepository playerRepository,
	IMapper mapper
	) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			foreach (var settlement in query.message.Txns)
			{
				var criteria = mapper.Map<SettleCriteria>(settlement);
				var spResult = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>
					(criteria, StoreProcedureMapper.SettleSP);

				if (spResult.SpResult != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResultAsync(query, spResult.SpResult);
				}

				//TODO if isOnlyWinlostDateChanged, then no need to recalculate params
				var playerCode = settlement.UserId.ToPlayerCode();

				var balanceResponse = await cashServiceClient.GameProviderAdjustBalanceAsync(
					playerCode,
					settlement.CreditAmount,
					settlement.DebitAmount,
					new Tgs.CashService.ProviderInfo(
						ProviderConst.ProviderCode,
						settlement.RefId,
						query.message.OperationId,
						query.message.Action),
					null,
					cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResultAsync(query, balanceResponse.ErrorCode);
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Resettle(criteria), cancellationToken);
			}

			return await GenerateResultAsync(query, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());

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