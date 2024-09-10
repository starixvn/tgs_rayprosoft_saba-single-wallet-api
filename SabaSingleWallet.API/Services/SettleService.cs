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
using Tgs.GameRecord.Saba.Common;
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

				if (result.StatusCode != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResultAsync(query, result.StatusCode);
				}

				if (transaction.CreditAmount > 0)
				{
					var balanceResponse = await cashClient.GameProviderAddBalanceAsync(
					transaction.UserId.ToPlayerCode(),
					transaction.CreditAmount,
					new Tgs.CashService.ProviderInfo(
						ProviderConst.ProviderCode,
						transaction.RefId,
						query.message.OperationId,
						query.message.Action
						),
					null,
					cancellationToken: cancellationToken);

					if (balanceResponse.ErrorCode != SabaStatusCodeConst.SuccessCode)
					{
						return await GenerateResultAsync(query, balanceResponse.ErrorCode);
					}
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Settle(criteria), cancellationToken);
			}

			return await GenerateResultAsync(query, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			SentrySdk.CaptureException(ex);
			return await GenerateResultAsync(query,SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private async Task<SettleResult> GenerateResultAsync(SettleRequest query, string status)
	{
		var res = new SettleResult(status);
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