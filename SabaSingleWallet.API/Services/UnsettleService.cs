using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.Unsettle;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

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
					new ProviderInfo(ProviderConst.ProviderCode, settlement.RefId, query.message.OperationId, query.message.Action),
					null,
					cancellationToken: cancellationToken);

				if (balanceResponse.ErrorCode != SabaStatusCodeConst.SuccessCode)
				{
					return await GenerateResultAsync(query, balanceResponse.ErrorCode);
				}

				await messagePublisher.HandleBetAsync(HandleMessageMapper.Unsettle(unSettleCriteria), cancellationToken);
			}

			return await GenerateResultAsync(query, SabaStatusCodeConst.SuccessCode);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return await GenerateResultAsync(query, SabaStatusCodeConst.ApiInternalErrorCode);
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