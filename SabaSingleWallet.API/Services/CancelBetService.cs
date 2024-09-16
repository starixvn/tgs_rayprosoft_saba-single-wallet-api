using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.CancelBet;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class CancelBetService(
	ISapaApiMessagePublishHandler messagePublisher,
	IPlayerRepository playerRepository,
	ICashServiceClient cashClient,
	ISentryClient sentryClient) : IRequestHandler<CancelBetRequest, CancelBetResult>

{
	public async Task<CancelBetResult> Handle(CancelBetRequest query, CancellationToken cancellationToken)
	{
		try
		{
			var playerCode = query.message.UserId.ToPlayerCode();
			var action = query.message.Action;
			var operationId = query.message.OperationId;
			var updateTime = DateTime.Parse(query.message.UpdateTime); //TODO review timezone

			foreach (var transaction in query.message.Txns)
			{
				var result = await playerRepository.CallStoredProcedureAsync<CancelBetCriteria, CancelBetSPResult>(new CancelBetCriteria
				{
					Action = action,
					OperationId = operationId,
					UpdateTime = updateTime,
					RefId = transaction.RefId,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
				}, StoreProcedureMapper.CancelBetSP);

				if (!CouldProcessNext(result.StatusCode))
				{
					return await GenerateResponseAsync(result.StatusCode, null, query, cancellationToken);
				}

				var isDeductPlaceBetAmount = await cashClient.IsExistGameProviderTransactionLogAsync(
					playerCode,
					new ProviderInfo(
						ProviderConst.ProviderCode,
						transaction.RefId,
						"PlaceBet"),
					cancellationToken);

				if (isDeductPlaceBetAmount)
				{
					var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
									playerCode,
									transaction.CreditAmount,
									new ProviderInfo(ProviderConst.ProviderCode, transaction.RefId, operationId, action),
									cancellationToken: cancellationToken);

					if (!CouldProcessNext(balanceResponse.ErrorCode))
					{
						return await GenerateResponseAsync(balanceResponse.ErrorCode, null, query, cancellationToken);
					}
				}
			}

			var balance = (await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

			return await GenerateResponseAsync(SabaStatusCodeConst.SuccessCode, balance, query, cancellationToken);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return await GenerateResponseAsync(SabaStatusCodeConst.ApiInternalErrorCode, null, query, cancellationToken);
		}
	}

	private static bool CouldProcessNext(string statusCode)
		=> statusCode == SabaStatusCodeConst.SuccessCode || SabaStatusCodeConst.IsDuplicatedError(statusCode);

	public async Task<CancelBetResult> GenerateResponseAsync(string status, decimal? balance, CancelBetRequest request, CancellationToken cancellationToken)
	{
		var res = new CancelBetResult(status, balance);

		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = request.key,
			Action = request.message.Action,
			Request = request,
			Response = res
		}, cancellationToken);

		return res;
	}
}