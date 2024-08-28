using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
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
				var criteria = new BetTransSBSWConfirmBetCriteria()
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
					WinLostDate = DateTimeOffset.Now.DateTime,
					UpdateTime = DateTimeOffset.Now.DateTime,
					TransactionTime = DateTimeOffset.Parse(confirmBetData.TransactionTime).DateTime,
				};

				var spCallResult = await playerRepository.CallBetTransSbswConfirmBetAsync(criteria);

				if (spCallResult.SpResult < 0)
				{
					continue;
				}

				var playerCode = confirmBetData.UserId.Replace("_test", string.Empty).Remove(0, confirmBetData.UserId.IndexOf("_") + 1);

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

			return new ConfirmBetResult("0", addAmountResults.Last().Balance, "");
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new ConfirmBetResult("1", 0, "");
		}
	}
}