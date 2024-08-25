using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetGetService(IApiLogsRepository apiLogsRepository) : IGetService<PlaceBetRequest, PlaceBetResult>
{
	public async ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new PlaceBetResult("0", "4200500_2331", "1988754", "");

			_ = await apiLogsRepository.AddAsync(query.message.Action, query, res, cancellationToken);

			//TO DO
			//#1. Check Valid Request
			//#2. Check Valid Bet Amount
			//#3. Check Valid Balance
			//#4. Define PlaceBet Error Codes
			//#5. Schedule to trigger CheckTicketStatus if not receive ConfirmBet after 10 mins

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetResult("1", "", "", "");
		}
	}
}