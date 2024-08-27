using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBetResult("0", 100, "");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			//TO DO
			//#1. Check Valid Request
			//#2. Check and update ActualAmount, Odds if isOddsChanged = true
			//#3. Define ConfirmBet Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new ConfirmBetResult("1", 0, "");
		}
	}
}