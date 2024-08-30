using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.GetTicketInfo;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class GetTicketInfoService(ISapaApiMessagePublishHandler messagePublisher,  IPlayerRepository playerRepository, ICashServiceClient cashClient) : IGetService<GetTicketInfoRequest, GetTicketInfoResult>
{
	public async ValueTask<GetTicketInfoResult> GetAsync(GetTicketInfoRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);

			var spResult = await playerRepository
				.CallStoredProcedureAsync<GetTicketInfoCriteria, GetTicketInfoSPResult>
				(new GetTicketInfoCriteria { 
					UserId = query.message.UserId,
					RefId = query.message.RefId,
				}, StoreProcedureMapper.GetTicketInfo);

			if (spResult is null )
			{
				return await GenerateResultAsync(query, new GetTicketInfoResult
				{
					Status = "506",
					Msg = "Unable To Processed. Please Try Again Later"
				});
			}
	
			return await GenerateResultAsync(query, new GetTicketInfoResult
			{
				Status = "0",
				TicketStatus = spResult.TicketStatus,
				ActualStake = spResult.ActualStake,
				WinlostAmount = spResult.WinlostAmount,
				Msg = "Success",
			} );
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return await GenerateResultAsync(query, new GetTicketInfoResult
			{
				Status = "506",
				Msg = "Unable To Processed. Please Try Again Later"
			});
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