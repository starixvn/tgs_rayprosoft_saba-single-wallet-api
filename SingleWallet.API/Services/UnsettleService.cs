using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class UnsettleService(IApiLogsRepository apiLogsRepository) : IGetService<UnsettleRequest, UnsettleResult>
{
	public async ValueTask<UnsettleResult> GetAsync(UnsettleRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new UnsettleResult("0");

			_ = await apiLogsRepository.AddAsync(query.message.Action, query, res, cancellationToken);

			//TO DO
			//#1. Validate Unsettlement: exist player, exist bet???
			//#2. Insert Unsettlement to DB
			//#3. Handle CreditAmount, DebitAmount
			//#4. Define Unsettlement Error Codes

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new UnsettleResult("803", "No Bet Found");
		}
	}
}