using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.GetTicketInfo;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class GetTicketInfoService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository, ICashServiceClient cashClient) : IGetService<GetTicketInfoRequest, GetTicketInfoResult>
{
	public async ValueTask<GetTicketInfoResult> GetAsync(GetTicketInfoRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			var result = await playerRepository
				.CallStoredProcedureAsync<GetTicketInfoCriteria, GetTicketInfoSPResult>
				(new GetTicketInfoCriteria
				{
					UserId = query.message.UserId,
					RefId = query.message.RefId,
				}, StoreProcedureMapper.GetTicketInfo);

			if (result.StatusCode != SabaStatusCodeConst.SuccessCode)
			{
				return await GenerateResultAsync(query, new GetTicketInfoResult(result.StatusCode));
			}

			return await GenerateResultAsync(query, new GetTicketInfoResult(
				result.StatusCode, result.TicketStatus, result.ActualStake, result.WinlostAmount
			));
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());

			return await GenerateResultAsync(query, new GetTicketInfoResult(SabaStatusCodeConst.ApiInternalErrorCode));
		}
	}

	private async Task<GetTicketInfoResult> GenerateResultAsync(GetTicketInfoRequest query, GetTicketInfoResult result)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = query.key,
			Action = query.message.Action,
			Request = query,
			Response = result
		});

		return result;
	}
}