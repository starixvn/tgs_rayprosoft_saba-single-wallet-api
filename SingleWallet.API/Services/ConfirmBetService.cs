using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetService(IApiLogsRepository apiLogsRepository) : IGetService<ConfirmBetRequest, ConfirmBetResult>
{
	public async ValueTask<ConfirmBetResult> GetAsync(ConfirmBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBetResult("0", 100, "");

			_ = await apiLogsRepository.AddAsync(query.message.Action, query, res, cancellationToken);

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