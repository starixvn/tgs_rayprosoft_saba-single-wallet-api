using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.CancelBet;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class CancelBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient cashClient) : IGetService<CancelBetRequest, CancelBetResult>
{
	public async ValueTask<CancelBetResult> GetAsync(CancelBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.message.UserId.ToPlayerCode();
			foreach (var transaction in query.message.Txns)
			{
				var result = await playerRepository.CallStoredProcedureAsync<CancelBetCriteria, CancelBetSPResult>(new CancelBetCriteria
				{
					Action = query.message.Action,
					OperationId = query.message.OperationId,
					UpdateTime = DateTime.Parse(query.message.UpdateTime),

					RefId = transaction.RefId,
					CreditAmount = transaction.CreditAmount,
					DebitAmount = transaction.DebitAmount,
				}, StoreProcedureMapper.CancelBetSP);

				if (result.StatusCode != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResponseAsync(result.StatusCode, null, query, cancellationToken);
				}

				var isDeductPlaceBetAmount = await cashClient.IsExistGameProviderTransactionLogAsync(
					playerCode,
					new Tgs.CashService.ProviderInfo(
						ProviderConst.ProviderCode,
						transaction.RefId,
						"PlaceBet"),
					cancellationToken);

				if (isDeductPlaceBetAmount)
				{
					var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
									playerCode,
									transaction.CreditAmount,
									new Tgs.CashService.ProviderInfo(
									ProviderConst.ProviderCode,
									transaction.RefId,
									query.message.OperationId,
									query.message.Action),
									cancellationToken: cancellationToken);

					if (balanceResponse.ErrorCode != SabaStatusCodeConst.SuccessCode)
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
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return await GenerateResponseAsync(SabaStatusCodeConst.ApiInternalErrorCode, null, query, cancellationToken);
		}
	}

	private async Task<CancelBetResult> GenerateResponseAsync(string status, decimal? balance, CancelBetRequest request, CancellationToken cancellationToken)
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