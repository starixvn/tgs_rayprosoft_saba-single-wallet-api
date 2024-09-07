using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetGetService(
		ICashServiceClient cashClient,
		ISapaApiMessagePublishHandler messagePublisher,
		IPlayerRepository playerRepository,
		IMapper mapper
	) : IGetService<PlaceBetRequest, PlaceBetResult>
{
	public async ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.message.UserId.ToPlayerCode();

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(
				playerCode,
				query.message.DebitAmount,
				new Tgs.CashService.ProviderInfo(
					ProviderConst.ProviderCode,
					query.message.RefId,
					query.message.OperationId,
					query.message.Action
					),
				null,
				cancellationToken
				).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(deductBalanceResult.ErrorMessage))
			{
				return await GenerateResponseAsync(query, new PlaceBetResult(deductBalanceResult.ErrorCode, query.message.RefId, null), cancellationToken);
			}

			var criteria = mapper.Map<PlaceBetCriteria>(query.message);
			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>(criteria, StoreProcedureMapper.PlaceBetSP);

			if (spResult.StatusCode != SabaStatusCodeConst.SuccessCode)
			{
				return await GenerateResponseAsync(query, new PlaceBetResult(spResult.StatusCode, query.message.RefId, null), cancellationToken);
			}

			var res = new PlaceBetResult(SabaStatusCodeConst.SuccessCode, query.message.RefId, spResult.Licenseetxid);

			return await GenerateResponseAsync(query, res, cancellationToken);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return await GenerateResponseAsync(query, new PlaceBetResult(SabaStatusCodeConst.ApiInternalErrorCode, query.message.RefId, null), cancellationToken);
		}
	}

	private async Task<PlaceBetResult> GenerateResponseAsync(PlaceBetRequest request, PlaceBetResult result, CancellationToken cancellationToken)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = request.key,
			Action = request.message.Action,
			Request = request,
			Response = result
		}, cancellationToken);

		return result;
	}
}