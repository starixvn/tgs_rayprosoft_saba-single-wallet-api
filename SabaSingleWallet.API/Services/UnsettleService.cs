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
using SabaSingleWallet.Core;
using SabaSingleWallet.API.Helpers;
using Tgs.GameRecord.Saba.Common;
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

				if (spResult.SpResult != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResultAsync(query, spResult.SpResult);
				}

				var playerCode = settlement.UserId.ToPlayerCode();
				var balanceResponse = await cashServiceClient.GameProviderAdjustBalanceAsync(
					playerCode,
					settlement.CreditAmount,
					settlement.DebitAmount,
					query.message.Action,
					query.message.OperationId,
					cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode > 0)
				{
					return await GenerateResultAsync(query, "307", "Cannot Credit player. Please Try Again Later");
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Unsettle(unSettleCriteria), cancellationToken);
			}

			return	await GenerateResultAsync(query, "0", string.Empty);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return await GenerateResultAsync(query, "803", "No Bet Found");
		}
	}
	private async Task<UnsettleResult> GenerateResultAsync(UnsettleRequest query, string status)
	{
		var res = new UnsettleResult(status);
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