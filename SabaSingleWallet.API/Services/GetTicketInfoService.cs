using System.Transactions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.API.Models.Placebetent;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.CancelBet;
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
					UserId = "Player",
					RefId = "171|$|64008486|$|3821|$|1165025",
				}, StoreProcedureMapper.GetTicketInfo);

			if (spResult is null )
			{
				return new GetTicketInfoResult
				{
					Status = "506",
					Msg = "Unable To Processed. Please Try Again Later"
				};
			}
			var res = new GetTicketInfoResult
			{
				Status = spResult.TicketStatus,
				TicketStatus = spResult.TicketStatus,
				ActualStake = spResult.ActualStake,
				WinlostAmount = spResult.WinlostAmount,
				Msg = "Success",
			};


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
			return new GetTicketInfoResult {
				Status = "506",
				Msg = "Unable To Processed. Please Try Again Later"
			};
		}
	}
}