using System.Text.Json;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Common;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace SabaSingleWallet.API.Services;

public class GetBalanceService(IMessageSender messageSender) : IGetService<GetBalanceRequest, GetBalanceResult>
{
	public async ValueTask<GetBalanceResult> GetAsync(GetBalanceRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new GetBalanceResult("0", "4200500_2331", 100, new DateTime());

			var apiLogMessage = new SabaSingleWalletApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			};
			await messageSender.PublishAsync(subject: SabaNatsConst.ApiLogsPublish, data: JsonSerializer.Serialize(apiLogMessage), cancellationToken);

			//TO DO
			//#1. Check Valid UserId
			//#2. Get Actual Balance
			//#3. Define GetBalance Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new GetBalanceResult("203", null, null, null, "Account not existed");
		}
	}
}