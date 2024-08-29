using AutoMapper;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.Repositories.Models.Unsettle;
using SabaSingleWallet.Repositories.Models.Settle;

namespace SabaSingleWallet.API.Services;

public class UnsettleService(ISapaApiMessagePublishHandler messagePublisher, 
	ICashServiceClient cashServiceClient, 
	IPlayerRepository playerRepository, 
	IMapper mapper) : IGetService<UnsettleRequest, UnsettleResult>
{
	public async ValueTask<UnsettleResult> GetAsync(UnsettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			foreach (var settlement in query.message.Txns)
				
			{
				var unSettleCriteria = mapper.Map<UnsettleCriteria>(settlement);
				unSettleCriteria.OperationId = query.message.OperationId;
				unSettleCriteria.Action = query.message.Action;
			
				var spResult = await playerRepository.CallStoredProcedureAsync<UnsettleCriteria, UnsettleSPResult>
					(unSettleCriteria, StoreProcedureMapper.SettleSP);

				if (spResult.SpResult < 0)
				{
					return new UnsettleResult("506", "Unable To Processed. Please Try Again Later");
				}

				var playerCode = settlement.UserId.ToPlayerCode();
				var adjustAmount = settlement.CreditAmount - settlement.DebitAmount;
				var balanceResponse = adjustAmount > 0
					? await cashServiceClient.GameProviderAddBalanceAsync(playerCode, adjustAmount, cancellationToken: cancellationToken)
					: await cashServiceClient.GameProviderDeductBalanceAsync(playerCode, -adjustAmount, cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode > 0)
				{
					return new UnsettleResult("506", "Unable To Processed. Please Try Again Later");
				}
			}

			var res = new UnsettleResult("0");

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
			return new UnsettleResult("803", "No Bet Found");
		}
	}
}