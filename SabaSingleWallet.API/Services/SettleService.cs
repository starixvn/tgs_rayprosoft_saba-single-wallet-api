using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class SettleService(
	ICashServiceClient cashClient,
	IPlayerRepository playerRepository,
	ISapaApiMessagePublishHandler messagePublisher,
	IMapper mapper,
	ISentryClient sentryClient) : IGetService<SettleRequest, SettleResult>
{
	public async ValueTask<SettleResult> GetAsync(SettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var action = query.message.Action;
			var operationId = query.message.OperationId;

			foreach (var transaction in query.message.Txns)
			{
				var criteria = mapper.Map<SettleCriteria>(transaction);
				criteria.OperationId = operationId;
				criteria.Action = action;

				var result = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>(criteria, StoreProcedureMapper.SettleSP);

				if (!CouldProcessNext(result.StatusCode))
				{
					return await GenerateResultAsync(query, result.StatusCode);
				}

				if (transaction.CreditAmount > 0)
				{
					var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
					transaction.UserId.ToPlayerCode(),
					transaction.CreditAmount,
					new ProviderInfo(ProviderConst.ProviderCode, transaction.RefId, operationId, action),
					cancellationToken: cancellationToken);

					if (!CouldProcessNext(result.StatusCode))
					{
						return await GenerateResultAsync(query, balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Settle(criteria), cancellationToken);
			}

			return await GenerateResultAsync(query, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return await GenerateResultAsync(query, SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private static bool CouldProcessNext(string statusCode)
		=> statusCode == SabaStatusCodeConst.SuccessCode || SabaStatusCodeConst.IsDuplicatedError(statusCode);

	private async Task<SettleResult> GenerateResultAsync(SettleRequest query, string status)
	{
		var res = new SettleResult(status);

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