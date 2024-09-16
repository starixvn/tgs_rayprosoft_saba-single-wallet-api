using AutoMapper;
using MediatR;
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

namespace SabaSingleWallet.API.Services;

public class ResettleService(
	ISapaApiMessagePublishHandler messagePublisher,
	ICashServiceClient cashServiceClient,
	IPlayerRepository playerRepository,
	IMapper mapper,
	ISentryClient sentryClient
	) : IRequestHandler<ResettleRequest, ResettleResult>
{
	public async Task<ResettleResult> Handle(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			foreach (var settlement in query.message.Txns)
			{
				var criteria = mapper.Map<ResettleCriteria>(settlement);
				criteria.OperationId = query.message.OperationId;
				criteria.Action = query.message.Action;

				var spResult = await playerRepository.CallStoredProcedureAsync<ResettleCriteria, ResettleSPResult>
					(criteria, StoreProcedureMapper.SettleSP);

				if (!CouldProcessNext(spResult.StatusCode))
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

					if (!CouldProcessNext(balanceResponse.ErrorCode))
					{
						return await GenerateResultAsync(query, balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Resettle(criteria), cancellationToken);
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