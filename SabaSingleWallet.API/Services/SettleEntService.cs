using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.SettleEnt;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;
namespace SabaSingleWallet.API.Services;

public class SettleEntService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<SettleEntRequest, SettleEntResult>
{
	public async ValueTask<SettleEntResult> GetAsync(SettleEntRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new SettleEntResult("0","success");

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
			return new SettleEntResult("803", "No Bet Found");
		}
	}
}