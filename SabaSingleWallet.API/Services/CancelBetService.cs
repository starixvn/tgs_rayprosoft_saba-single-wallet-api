using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.CancelBet;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class CancelBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient cashClient) : IGetService<CancelBetRequest, CancelBetResult>
{
	public async ValueTask<CancelBetResult> GetAsync(CancelBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			var transaction = query.message.Txns.FirstOrDefault();

			var result = await playerRepository.CallBetTransSbswCancelBetAsync(new CancelBetCriteria
			{
				Action = query.message.Action,
				OperationId = query.message.OperationId,
				UpdateTime = DateTime.Parse(query.message.UpdateTime),
				RefId = transaction?.RefId,
				CreditAmount = transaction.CreditAmount,
				DebitAmount = transaction.DebitAmount,
				SpResult = 0,
				SportType = null
			});
			
			if ( result.SpResult < 0 )
			{
				return new CancelBetResult("1", 0, "cancel failed");
			}

			var cashResponse = await cashClient
					.GameProviderAddBalanceAsync(query.message.UserId, transaction.CreditAmount);

			var res = new CancelBetResult("200", cashResponse.Balance, "cancel success");

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
			return new CancelBetResult("1", 0, "cancel failed");
		}
	}
}