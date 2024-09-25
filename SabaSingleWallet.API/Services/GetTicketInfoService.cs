using MediatR;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.GetTicketInfo;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class GetTicketInfoService(
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
				return new GetTicketInfoResult(result.StatusCode);
			}

			return new GetTicketInfoResult(
				result.StatusCode, result.TicketStatus, result.ActualStake, result.WinlostAmount);
		}
		catch (Exception ex)
		{
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());

			return new GetTicketInfoResult(SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}
}