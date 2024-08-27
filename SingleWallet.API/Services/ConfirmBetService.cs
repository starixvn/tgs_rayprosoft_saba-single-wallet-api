using System.Text.Json;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(IMessageSender messageSender) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBetResult("0", 100, "");

			var apiLogMessage = new SabaSingleWalletApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			};
			await messageSender.PublishAsync(subject: SabaNatsConst.ApiLogsPublish, data: JsonSerializer.Serialize(apiLogMessage), cancellationToken);

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