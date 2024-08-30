using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient client) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		Console.WriteLine(query.message);

		var response = await ProcessRequestAsync(query, cancellationToken);

		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = query.key,
			Action = query.message.Action,
			Request = query,
			Response = response
		}, cancellationToken);

		return response;
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
					WinLostDate = DateTimeOffset.Parse(transaction.WinlostDate).DateTime,
					TransactionTime = DateTimeOffset.Parse(confirmBetData.TransactionTime).DateTime,
					UpdateTime = DateTimeOffset.Parse(confirmBetData.UpdateTime).DateTime,
				};

				var spCallResult = await playerRepository.CallStoredProcedureAsync<ConfirmBetCriteria, ConfirmBetSPResult>(criteria, StoreProcedureMapper.ConfirmBetSP);

				if (spCallResult.SpResult < 0)
				{
					return await GenerateResponseAsync("506", 0, "Unable To Processed. Please Try Again Later", query, cancellationToken);
				}

				if (transaction.IsOddsChanged)
				{
					var addAmmountResut = await client.GameProviderAddBalanceAsync(playerCode, transaction.CreditAmount, cancellationToken: cancellationToken);

					if (!string.IsNullOrWhiteSpace(addAmmountResut.ErrorMessage))
					{
						return await GenerateResponseAsync("307", 0, "Cannot Credit player. Please Try Again Later", query, cancellationToken);
					}
				}

				var balance = (await client.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

				await messagePublisher.HandleBetAsync(HandleMessageMapper.ConfirmBet(criteria, playerCode), cancellationToken);

				return await GenerateResponseAsync("0", balance, "", query, cancellationToken);
			}

			return await GenerateResponseAsync("504", 0, "No Such Ticket", query, cancellationToken);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return await GenerateResponseAsync("999", 0, "System Error", query, cancellationToken);
		}
	}

	private async Task<ConfirmBetResult> GenerateResponseAsync(string status, decimal balance, string? message, ConfirmBetRequest request, CancellationToken cancellationToken)
	{
		var res = new ConfirmBetResult(status, balance, message);

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