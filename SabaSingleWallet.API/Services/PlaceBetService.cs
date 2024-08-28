using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetGetService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<PlaceBetRequest, PlaceBetResult>
{
	public async ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var licenseeTxId = Guid.NewGuid().ToString();
			var res = new PlaceBetResult("0", query.message.RefId, licenseeTxId, "");

			var apiLogMessage = new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			};

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			//TO DO
			//#1. Check Valid Request
			//#2. Check Valid Bet Amount
			//#3. Check Valid Balance
			//#4. Define PlaceBet Error Codes
			//#5. Schedule to trigger CheckTicketStatus if not receive ConfirmBet after 10 mins

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetResult("1", "", "", "");
		}
	}
}