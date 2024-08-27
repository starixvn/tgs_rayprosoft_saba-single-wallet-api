using System.Text.Json;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Services;

public class CancelBetService(IMessageSender messageSender) : IGetService<CancelBetRequest, CancelBetResult>
{
	public async ValueTask<CancelBetResult> GetAsync(CancelBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new CancelBetResult("0", 100, "");

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