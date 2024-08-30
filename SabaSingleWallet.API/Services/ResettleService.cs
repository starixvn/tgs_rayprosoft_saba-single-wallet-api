using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
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
				var criteria = mapper.Map<SettleCriteria>(settlement);
				var spResult = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>
					(criteria, StoreProcedureMapper.SettleSP);

				if (spResult.SpResult < 0)
				{
					return await GenerateResultAsync(query, "506", "Unable To Processed. Please Try Again Later (SP)");
				}

				//TODO if isOnlyWinlostDateChanged, then no need to recalculate params

				var playerCode = settlement.UserId.ToPlayerCode();
				var adjustAmount = settlement.CreditAmount - settlement.DebitAmount;

				var balanceResponse = adjustAmount > 0
					? await cashServiceClient.GameProviderAddBalanceAsync(playerCode, adjustAmount, cancellationToken: cancellationToken)
					: await cashServiceClient.GameProviderDeductBalanceAsync(playerCode, -adjustAmount, cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode > 0)
				{
					return await GenerateResultAsync(query, "307", "Cannot Credit player. Please Try Again Later");
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Resettle(criteria), cancellationToken);
			}

			return new ResettleResult("0");
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return await GenerateResultAsync(query, "803", "No Bet Found");
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