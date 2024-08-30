using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Core.Models.Criteria.GameBet;
using SabaSingleWallet.Core.Models.Criteria.GameRound;
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

			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>
				(
				mapper.Map<PlaceBetCriteria>(query.message),
				StoreProcedureMapper.PlaceBetSP
				);

			if (spResult.SpResult != 0)
			{
				return new PlaceBetResult("506", query.message.RefId, null, "Unable To Processed. Please Try Again Later");
			}

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(playerCode, query.message.DebitAmount).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(deductBalanceResult.ErrorMessage))
			{
				return new PlaceBetResult("506", query.message.RefId, null, "Unable To Processed. Please Try Again Later");
			}

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
			return new PlaceBetResult("503", query.message.RefId, null, "Internal Error");
		}
	}
}