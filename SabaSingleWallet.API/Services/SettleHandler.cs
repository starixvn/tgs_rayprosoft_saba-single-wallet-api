using AutoMapper;
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class SettleHandler(
	ICashServiceClient cashClient,
	IPlayerRepository playerRepository,
	ISapaApiMessagePublishHandler messagePublisher,
	IMapper mapper,
	ISentryClient sentryClient) : IRequestHandler<SettleRequest, SettleResult>
{
	public async Task<SettleResult> Handle(SettleRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var action = request.message.Action;
			var operationId = request.message.OperationId;

			foreach (var transaction in request.message.Txns)
			{
				var criteria = mapper.Map<SettleCriteria>(transaction);
				criteria.OperationId = operationId;
				criteria.Action = action;

				var result = await playerRepository.CallStoredProcedureAsync< SettleSPResult>(criteria);

				if (!CouldProcessNext(result.StatusCode))
				{
					return await GenerateResultAsync(request, result.StatusCode);
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
						return await GenerateResultAsync(request, balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Settle(criteria), cancellationToken);
			}

			return await GenerateResultAsync(request, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return await GenerateResultAsync(request, SabaStatusCodeConst.ApiInternalErrorCode);
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