using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class CancelBetService(ISapaApiMessagePublishHandler messagePublisher, IPlayerRepository playerRepository) : IGetService<CancelBetRequest, CancelBetResult>
{
	public async ValueTask<CancelBetResult> GetAsync(CancelBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new CancelBetResult("0", 100, "");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);
			var result = await playerRepository.CallBetTransSbswCancelBetAsync();
			//TO DO
			//#1. Check Valid Request
			//#2. Check and update creditAmount, debitAmount
			//#3. Define CancelBet Error Codes
	
			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new CancelBetResult("1", 0, "");
		}
	}
}