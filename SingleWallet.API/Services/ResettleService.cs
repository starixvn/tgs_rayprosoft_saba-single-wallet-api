using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ResettleService(IApiLogsRepository apiLogsRepository) : IGetService<ResettleRequest, ResettleResult>
{
	public async ValueTask<ResettleResult> GetAsync(ResettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ResettleResult("0");

			_ = await apiLogsRepository.AddAsync(query.message.Action, query, res, cancellationToken);

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