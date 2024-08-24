using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class GetBalanceService(IGetBalanceLogsRepository getBalanceLogsRepository) : IGetService<GetBalanceRequest, GetBalanceResult>
{
	public async ValueTask<GetBalanceResult> GetAsync(GetBalanceRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new GetBalanceResult("0", "4200500_2331", 100, new DateTime());

			_ = await getBalanceLogsRepository.AddAsync(query, res, cancellationToken);

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