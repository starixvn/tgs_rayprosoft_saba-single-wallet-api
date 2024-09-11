using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient client) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		Console.WriteLine(query.message);

		return await ProcessRequestAsync(query, cancellationToken);
	}

	private async Task<ConfirmBetResult> ProcessRequestAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var confirmBetData = query.message;
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

				var spCallResult = await playerRepository.CallStoredProcedureAsync<ConfirmBetCriteria, ConfirmBetSPResult>(criteria, StoreProcedureMapper.ConfirmBetSP);

				if (spCallResult.StatusCode != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResponseAsync(spCallResult.StatusCode, 0, query, cancellationToken);
				}

				if (transaction.IsOddsChanged && transaction.CreditAmount > 0)
				{
					var addAmmountResut = await client.GameProviderAddBalanceAsync(
						playerCode,
						transaction.CreditAmount,
						new Tgs.CashService.ProviderInfo(
						ProviderConst.ProviderCode,
						transaction.RefId,
						query.message.OperationId,
						query.message.Action),
						cancellationToken: cancellationToken);

					if (addAmmountResut.ErrorCode != SabaStatusCodeConst.SuccessCode)
					{
						return await GenerateResponseAsync(addAmmountResut.ErrorCode, 0, query, cancellationToken);
					}
				}

				var balance = (await client.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

				await messagePublisher.HandleBetAsync(HandleMessageMapper.ConfirmBet(criteria, playerCode), cancellationToken);

				return await GenerateResponseAsync(SabaStatusCodeConst.SuccessCode, balance, query, cancellationToken);
			}

			return await GenerateResponseAsync(SabaStatusCodeConst.NoTransactionFound, 0, query, cancellationToken);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return await GenerateResponseAsync(SabaStatusCodeConst.ApiInternalErrorCode, 0, query, cancellationToken);
		}
	}

	private async Task<ConfirmBetResult> GenerateResponseAsync(string status, decimal balance, ConfirmBetRequest request, CancellationToken cancellationToken)
	{
		var res = new ConfirmBetResult(status, balance);

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