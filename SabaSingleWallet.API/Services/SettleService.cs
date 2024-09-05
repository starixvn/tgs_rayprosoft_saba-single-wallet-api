using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class SettleService(
	ICashServiceClient cashClient,
	IPlayerRepository playerRepository,
	ISapaApiMessagePublishHandler messagePublisher,
	IMapper mapper) : IGetService<SettleRequest, SettleResult>
{
	public async ValueTask<SettleResult> GetAsync(SettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			foreach (var transaction in query.message.Txns)
			{
				var criteria = mapper.Map<SettleCriteria>(transaction);
				criteria.OperationId = query.message.OperationId;
				criteria.Action = query.message.Action;
				var result = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>(criteria, StoreProcedureMapper.SettleSP);

				if (result.SpResult < 1)
				{
					return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later");
				}

				if (transaction.CreditAmount > 0)
				{
					var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
					transaction.UserId.ToPlayerCode(),
					transaction.CreditAmount,
					cancellationToken: cancellationToken);

					if (!string.IsNullOrWhiteSpace(balanceResponse.ErrorMessage))
					{
						return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later");
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Settle(criteria), cancellationToken);
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
			SentrySdk.CaptureException(ex);
			return await GenerateResultAsync(query, "803", "No Bet Found");
		}
	}

	private async Task<SettleResult> GenerateResultAsync(SettleRequest query, string status, string msg)
	{
		var res = new SettleResult(status, msg);
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = query.key,
			Action = query.message.Action,
			Request = query,
			Response = res
		});

		return res;
	}
}