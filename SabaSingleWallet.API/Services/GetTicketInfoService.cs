using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.GetTicketInfo;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class GetTicketInfoService(
	ISapaApiMessagePublishHandler messagePublisher,
	IPlayerRepository playerRepository
	) : IRequestHandler<GetTicketInfoRequest, GetTicketInfoResult>
{
	public async Task<GetTicketInfoResult> Handle(GetTicketInfoRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);

			var result = await playerRepository
				.CallStoredProcedureAsync<GetTicketInfoSPResult>
				(new GetTicketInfoCriteria
				{
					UserId = query.message.UserId,
					RefId = query.message.RefId,
				});

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