using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class SettleService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<SettleRequest, SettleResult>
{
	public async ValueTask<SettleResult> GetAsync(SettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new SettleResult("0");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			//TO DO
			//#1. Validate Settlement: exist player, exist bet???
			//#2. Insert Settlement to DB
			//#3. Define Settle Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new SettleResult("803", "No Bet Found");
		}
	}
}