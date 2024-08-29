using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Settle;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ResettleService(
	ISapaApiMessagePublishHandler messagePublisher,
	ICashServiceClient cashServiceClient,
	IPlayerRepository playerRepository,
	IMapper mapper
	) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			foreach (var settlement in query.message.Txns)
			{
				var spResult = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>
					(mapper.Map<SettleCriteria>(settlement), StoreProcedureMapper.SettleSP);

				if (spResult.SpResult < 0)
				{
					return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later (SP)");
				}

				var playerCode = settlement.UserId.ToPlayerCode();
				var adjustAmount = settlement.CreditAmount - settlement.DebitAmount;

				var balanceResponse = adjustAmount > 0
					? await cashServiceClient.GameProviderAddBalanceAsync(playerCode, adjustAmount, cancellationToken: cancellationToken)
					: await cashServiceClient.GameProviderDeductBalanceAsync(playerCode, -adjustAmount, cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode > 0)
				{
					return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later (CSC)");
				}
			}

			return new ResettleResult("0");
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later (500)");
		}
	}

	private async Task<ResettleResult> GenerateResultAsync(ResettleRequest query, string status, string msg)
	{
		var res = new ResettleResult(status, msg);
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