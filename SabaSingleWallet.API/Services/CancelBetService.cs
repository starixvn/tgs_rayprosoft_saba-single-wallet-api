using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
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
			foreach (var transaction in query.message.Txns)
			{
				var result = await playerRepository.CallStoredProcedureAsync<CancelBetCriteria, CancelBetSPResult>(new CancelBetCriteria
				{
					Action = query.message.Action,
					OperationId = query.message.OperationId,
					UpdateTime = DateTime.Parse(query.message.UpdateTime),
					RefId = transaction?.RefId,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
					SpResult = 0,
					SportType = null
				}, StoreProcedureMapper.CancelBetSP);

				if (result.SpResult < 0)
				{
					return new CancelBetResult("506", null, "Unable To Processed. Please Try Again Later");
				}

				var balanceResponse = await cashClient
						.GameProviderAddBalanceAsync(
					query.message.UserId.ToPlayerCode(),
					transaction.CreditAmount,
					cancellationToken: cancellationToken);

				if (!string.IsNullOrWhiteSpace(balanceResponse.ErrorMessage))
				{
					return new CancelBetResult("506", null, "Unable To Processed. Please Try Again Later");
				}
			}

			var res = new CancelBetResult("0");

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
			return new CancelBetResult("803", null, "No Bet Found");
		}
	}
}