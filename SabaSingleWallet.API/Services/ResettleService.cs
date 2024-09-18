using AutoMapper;
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
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
	public async Task<ResettleResult> Handle(ResettleRequest query, CancellationToken cancellationToken)
	{
		try
		{
			foreach (var settlement in query.message.Txns)
			{
				var criteria = mapper.Map<ResettleCriteria>(settlement);
				criteria.OperationId = query.message.OperationId;
				criteria.Action = query.message.Action;

				var spResult = await playerRepository.CallStoredProcedureAsync<ResettleSPResult>(criteria);

				if (!CouldProcessNext(spResult.StatusCode))
				{
					return new ResettleResult(spResult.StatusCode);
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
						return new ResettleResult(balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Resettle(criteria), cancellationToken);
			}

			return new ResettleResult(SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return new ResettleResult(SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private static bool CouldProcessNext(string statusCode)
		=> statusCode == SabaStatusCodeConst.SuccessCode || SabaStatusCodeConst.IsDuplicatedError(statusCode);
}