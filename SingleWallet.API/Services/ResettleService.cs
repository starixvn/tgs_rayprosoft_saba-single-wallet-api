using System.Text.Json;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Services;

public class ResettleService(IMessageSender messageSender) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ResettleResult("0");

			var apiLogMessage = new SabaSingleWalletApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			};
			await messageSender.PublishAsync(subject: SabaNatsConst.ApiLogsPublish, data: JsonSerializer.Serialize(apiLogMessage), cancellationToken);

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