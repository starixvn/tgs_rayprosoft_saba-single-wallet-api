using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
using Tgs.GameRecord.Handle.Common.Protos;
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

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(playerCode, query.message.DebitAmount, query.message.Action, query.message.OperationId).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(deductBalanceResult.ErrorMessage))
			{
				return await GenerateResponseAsync(query, new PlaceBetResult("307", query.message.RefId, null, "Cannot Credit player. Please Try Again Later"), cancellationToken);
			}

			var criteria = mapper.Map<PlaceBetCriteria>(query.message);
			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>(criteria, StoreProcedureMapper.PlaceBetSP);

			if (spResult.SpResult != 0)
			{
				return await GenerateResponseAsync(query, new PlaceBetResult("506", query.message.RefId, null, "Unable To Processed. Please Try Again Later"), cancellationToken);
			}

			var res = new PlaceBetResult("0", query.message.RefId, spResult.Licenseetxid, "");

			return await GenerateResponseAsync(query, res, cancellationToken);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return await GenerateResponseAsync(query, new PlaceBetResult("803", query.message.RefId, null, "No Bet Found"), cancellationToken);
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