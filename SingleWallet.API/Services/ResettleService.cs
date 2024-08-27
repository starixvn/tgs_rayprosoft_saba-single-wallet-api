using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ResettleService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ResettleResult("0");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			//TO DO
			//#1. Validate Settlement: exist player, exist bet???
			//#2. Insert Re-Settlement to DB
			//#2. Handle creditAmount, debitAmount, payout
			//#3. Define ReSettle Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new ResettleResult("803", "No Bet Found");
		}
	}
}