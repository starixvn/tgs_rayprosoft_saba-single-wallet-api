using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class SettleService(ISettleLogsRepository SettleLogsRepository) : IGetService<SettleRequest, SettleResult>
{
	public async ValueTask<SettleResult> GetAsync(SettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new SettleResult("0");

			_ = await SettleLogsRepository.AddAsync(query, res, cancellationToken);

			//TO DO
			//#1. Validate Settlement: exist player, exist bet???
			//#2. Insert Settlement to DB
			//#3. Define Settle Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new SettleResult("803", "No Bet Found");
		}
	}
}