using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class SettleService(
	ICashServiceClient cashClient,
	IPlayerRepository playerRepository,
	ISapaApiMessagePublishHandler messagePublisher) : IGetService<SettleRequest, SettleResult>
{
	public async ValueTask<SettleResult> GetAsync(SettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			foreach (var transaction in query.message.Txns)
			{
				var result = await playerRepository.SettleAsync(new SettleCriteria
				{
					Action = query.message.Action,
					OperationId = query.message.OperationId,
					UserId = transaction.UserId,
					RefId = transaction.RefId,
					TxId = transaction.TxId,
					UpdateTime = DateTime.Parse(transaction.UpdateTime),
					WinlostDate = DateTime.Parse(transaction.WinlostDate),
					Status = transaction.Status,
					Payout = transaction.Payout,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
					ExtraStatus = transaction.ExtraStatus,
					TicketType = 0
				});

				if (result.SpResult < 1)
				{
					return new SettleResult("506", "Unable To Processed. Please Try Again Later");
				}

				var playerCode = transaction.UserId.Replace("_test", string.Empty).Remove(0, transaction.UserId.IndexOf("_") + 1);
				var balanceResponse = await cashClient.GameProviderAddBalanceAsync(playerCode, transaction.CreditAmount, cancellationToken: cancellationToken);

				if (!string.IsNullOrWhiteSpace(balanceResponse.ErrorMessage))
				{
					return new SettleResult("506", "Unable To Processed. Please Try Again Later");
				}
			}

			var res = new SettleResult("0");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new SettleResult("803", "No Bet Found");
		}
	}
}