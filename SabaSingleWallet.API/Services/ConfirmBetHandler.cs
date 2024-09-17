using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetHandler(
	ISapaApiMessagePublishHandler messagePublisher,
	IPlayerRepository playerRepository,
	ICashServiceClient client,
	ISentryClient sentryClient)
	: IRequestHandler<ConfirmBetRequest, ConfirmBetResult>
{
	public async Task<ConfirmBetResult> Handle(ConfirmBetRequest request, CancellationToken cancellationToken)
	{
		return await ProcessRequestAsync(request, cancellationToken);
	}

	private async Task<ConfirmBetResult> ProcessRequestAsync(ConfirmBetRequest request, CancellationToken cancellationToken = default)
	{
		try
		{
			var confirmBetData = request.message;
			var transaction = confirmBetData.Txns.FirstOrDefault();
			var playerCode = confirmBetData.UserId.ToPlayerCode();

			if (transaction != null)
			{
				var criteria = new ConfirmBetCriteria
				{
					Action = confirmBetData.Action,
					OperationId = confirmBetData.OperationId,
					ActualAmount = transaction.ActualAmount,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
					IsOddsChanged = transaction.IsOddsChanged,
					LicenseetxId = long.Parse(transaction.LicenseeTxId),
					Odds = transaction.Odds,
					OddsType = transaction.OddsType,
					RefId = transaction.RefId,
					TxId = transaction.TxId,
					WinLostDate = DateTimeOffset.Parse(transaction.WinlostDate).ToUniversalTime().DateTime,
					TransactionTime = DateTimeOffset.Parse(confirmBetData.TransactionTime).ToUniversalTime().DateTime,
					UpdateTime = DateTimeOffset.Parse(confirmBetData.UpdateTime).ToUniversalTime().DateTime,
				};

				var spCallResult = await playerRepository.CallStoredProcedureAsync<ConfirmBetSPResult>(criteria);

				if (spCallResult.StatusCode != SabaStatusCodeConst.SuccessCode)
				{
					return new ConfirmBetResult(spCallResult.StatusCode, 0);
				}

				if (transaction.IsOddsChanged && transaction.CreditAmount > 0)
				{
					var addAmmountResut = await client.GameProviderAddBalanceAsync(
						playerCode,
						transaction.CreditAmount,
						new Tgs.CashService.ProviderInfo(
						ProviderConst.ProviderCode,
						transaction.RefId,
						request.message.OperationId,
						request.message.Action),
						cancellationToken: cancellationToken);

					if (addAmmountResut.ErrorCode != SabaStatusCodeConst.SuccessCode)
					{
						return new ConfirmBetResult(addAmmountResut.ErrorCode, 0);
					}
				}

				var balance = (await client.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

				await messagePublisher.HandleBetAsync(HandleMessageMapper.ConfirmBet(criteria, playerCode), cancellationToken);

				return new ConfirmBetResult(SabaStatusCodeConst.SuccessCode, balance);
			}

			return new ConfirmBetResult(SabaStatusCodeConst.NoTransactionFound, 0);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
			return new ConfirmBetResult(SabaStatusCodeConst.ApiInternalErrorCode, 0);
		}
	}
}