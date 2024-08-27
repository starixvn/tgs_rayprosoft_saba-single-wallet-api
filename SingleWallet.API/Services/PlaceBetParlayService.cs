using System.Text.Json;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Services;

public class PlaceBetParlayService(IMessageSender messageSender) : IGetService<PlaceBetParlayRequest, PlaceBetParlayResult>
{
	public async ValueTask<PlaceBetParlayResult> GetAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new PlaceBetParlayResult("0", [], "");

			var apiLogMessage = new SabaSingleWalletApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			};
			await messageSender.PublishAsync(subject: SabaNatsConst.ApiLogsPublish, data: JsonSerializer.Serialize(apiLogMessage), cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetParlayResult("203", null, "");
		}
	}
}