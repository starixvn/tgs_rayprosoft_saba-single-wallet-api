using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetParlayService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<PlaceBetParlayRequest, PlaceBetParlayResult>
{
	public async ValueTask<PlaceBetParlayResult> GetAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new PlaceBetParlayResult("0", [], "");

			//await messagePublisher.LogApi(new ApiLogMessage()
			//{
			//	Key = query.key,
			//	Action = query.message.Action,
			//	Request = query,
			//	Response = res
			//}, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetParlayResult("203", null, "");
		}
	}
}