using AutoMapper;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
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
			Console.WriteLine(query.message);
			var res = new ResettleResult("0");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			foreach (var settlement in query.message.Txns)
			{
				var spResult = await playerRepository.CallStoredProcedureAsync<SettleCriteria, SettleSPResult>
					(mapper.Map<SettleCriteria>(settlement), StoreProcedureMapper.Settle_SP);

				if(spResult.SpResult == 0)
				{
					var playerCode = PlayerCodeHelpers.GetPlayerCode(settlement.UserId);

					if(settlement.DebitAmount > 0)
					{
						await cashServiceClient.GameProviderAddBalanceAsync(playerCode, settlement.DebitAmount);
					}

					if(settlement.CreditAmount > 0)
					{
						await cashServiceClient.GameProviderDeductBalanceAsync(playerCode, settlement.CreditAmount);
					}
				}
			}

			return res;
		}
		catch (Exception ex)
		{
			var res = new ResettleResult("803", "No Bet Found");
			Console.Write(ex.ToString());
			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			return res;
		}
	}
}