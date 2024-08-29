using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient client) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			var confirmBetData = query.message;
			var addAmountResults = new List<BalanceResponse>();
			foreach (var ticket in confirmBetData.Txns)
			{
				var spCallResult = await playerRepository.CallStoredProcedureAsync<ConfirmBetCriteria, ConfirmBetSPResult>(new ConfirmBetCriteria
				{
					Action = confirmBetData.Action,
					ActualAmount = ticket.ActualAmount,
					CreditAmount = ticket.CreditAmount,
					DebitAmount = ticket.DebitAmount,
					IsOddsChanged = ticket.IsOddsChanged,
					LicenseetxId = long.Parse(ticket.LicenseeTxId),
					Odds = ticket.Odds,
					OddsType = ticket.OddsType,
					OperationId = confirmBetData.OperationId,
					RefId = ticket.RefId,
					TxId = ticket.TxId,
					WinLostDate = DateTimeOffset.Parse(ticket.WinlostDate).DateTime,
					TransactionTime = DateTimeOffset.Parse(confirmBetData.TransactionTime).DateTime,
					UpdateTime = DateTimeOffset.Parse(confirmBetData.UpdateTime).DateTime,
				}, StoreProcedureMapper.ConfirmBetSP);

				if (spCallResult.SpResult < 0)
				{
					continue;
				}

				if (!ticket.IsOddsChanged)
				{
					continue;
				}

				var playerCode = PlayerCodeHelpers.GetPlayerCodeFromUserId(confirmBetData.UserId);

				var addAmmountResut = await client.GameProviderAddBalanceAsync(playerCode, ticket.CreditAmount, "", null, null, cancellationToken);
				addAmountResults.Add(addAmmountResut);
			}

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = addAmountResults
			}, cancellationToken);

			//TO DO
			//#1. Check Valid Request
			//#2. Check and update ActualAmount, Odds if isOddsChanged = true
			//#3. Define ConfirmBet Error Codes

			return addAmountResults.Count == 0
				? new ConfirmBetResult("506", 0, "Unable To Processed. Please Try Again Later")
				: new ConfirmBetResult("0", addAmountResults.Last().Balance, "");
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new ConfirmBetResult("506", 0, "Unable To Processed. Please Try Again Later");
		}
	}
}