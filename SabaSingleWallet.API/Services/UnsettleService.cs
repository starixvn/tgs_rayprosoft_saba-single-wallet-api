using AutoMapper;
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.Unsettle;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class UnsettleService(ISapaApiMessagePublishHandler messagePublisher,
	ICashServiceClient cashServiceClient,
	IPlayerRepository playerRepository,
	IMapper mapper,
	ISentryClient sentryClient) : IRequestHandler<UnsettleRequest, UnsettleResult>
{
	public async Task<UnsettleResult> Handle(UnsettleRequest query, CancellationToken cancellationToken)
	{
		try
		{
			foreach (var settlement in query.message.Txns)

			{
				var unSettleCriteria = mapper.Map<UnsettleCriteria>(settlement);
				unSettleCriteria.OperationId = query.message.OperationId;
				unSettleCriteria.Action = query.message.Action;
				var spResult = await playerRepository.CallStoredProcedureAsync<UnsettleSPResult>(unSettleCriteria);

				if (spResult.StatusCode != SabaStatusCodeConst.SuccessCode)
				{
					return new UnsettleResult(spResult.StatusCode);
				}

				var playerCode = settlement.UserId.ToPlayerCode();
				var balanceResponse = await cashServiceClient.GameProviderAdjustBalanceAsync(
					playerCode,
					settlement.CreditAmount,
					settlement.DebitAmount,
					new ProviderInfo(ProviderConst.ProviderCode, settlement.RefId, query.message.OperationId, query.message.Action),
					null,
					cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode != SabaStatusCodeConst.SuccessCode)
				{
					return new UnsettleResult(balanceResponse.ErrorCode);
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Unsettle(unSettleCriteria), cancellationToken);
			}

			return new UnsettleResult(SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return new UnsettleResult(SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}
}