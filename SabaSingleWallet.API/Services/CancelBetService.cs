using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
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
					SpResult = 0,
					SportType = null
				}, StoreProcedureMapper.CancelBetSP);

				if (result.SpResult < 0)
				{
					return await GenerateResponseAsync("506", 0, "Unable To Processed. Please Try Again Later", query, cancellationToken);
				}

				var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
					playerCode,
					transaction.CreditAmount,
					cancellationToken: cancellationToken);

				if (!string.IsNullOrWhiteSpace(balanceResponse.ErrorMessage))
				{
					return await GenerateResponseAsync("307", 0, "Cannot Credit player. Please Try Again Later", query, cancellationToken);
				}
			}

			var balance = (await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken)).Balance;

			return await GenerateResponseAsync("0", balance, "", query, cancellationToken);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return new CancelBetResult("803", null, "No Bet Found");
		}
	}

	private async Task<CancelBetResult> GenerateResponseAsync(string status, decimal balance, string? message, CancelBetRequest request, CancellationToken cancellationToken)
	{
		var res = new CancelBetResult(status, balance, message);

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