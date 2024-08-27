using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBetParlay;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetParlayService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<ConfirmBetParlayRequest, ConfirmBetParlayResult>
{
	public async ValueTask<ConfirmBetParlayResult> GetAsync(ConfirmBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBetParlayResult("0", 100, "");

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
			return new ConfirmBetParlayResult("1", 0, "");
		}
	}
}
