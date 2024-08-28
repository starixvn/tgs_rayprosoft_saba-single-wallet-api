using AutoMapper;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
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
			var playerCode = PlayerCodeHelpers.GetPlayerCode(query.message.UserId);

			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>
				(
				mapper.Map<PlaceBetCriteria>(query.message),
				StoreProcedureMapper.PlaceBetSP
				);

			if (spResult.SpResult != 0)
			{
				return new PlaceBetResult("1", query.message.RefId, null, "place bet failed");
			}

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(playerCode, query.message.DebitAmount).ConfigureAwait(false);

			// TODO: - Validate result

			var res = new PlaceBetResult("0", query.message.RefId, spResult.Licenseetxid, "");

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
			return new PlaceBetResult("1", "", "", "");
		}
	}
}