using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
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
				var result = await playerRepository.CallStoredProcedureAsync<CancelBetSPResult>(new CancelBetCriteria
				{
					Action = action,
					OperationId = operationId,
					UpdateTime = updateTime,
					RefId = transaction.RefId,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
				});

				if (!CouldProcessNext(result.StatusCode))
				{
					return new CancelBetResult(result.StatusCode, 0);
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
						return new CancelBetResult(balanceResponse.ErrorCode, 0);
					}
				}
			}

			var balance = (await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

			return new CancelBetResult(SabaStatusCodeConst.SuccessCode, balance);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);

			return new CancelBetResult(SabaStatusCodeConst.ApiInternalErrorCode, 0);
		}
	}

	private static bool CouldProcessNext(string statusCode)
		=> statusCode == SabaStatusCodeConst.SuccessCode || SabaStatusCodeConst.IsDuplicatedError(statusCode);
}