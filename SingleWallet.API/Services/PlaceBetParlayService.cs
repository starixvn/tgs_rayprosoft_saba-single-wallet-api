using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetParlayService(IApiLogsRepository apiLogsRepository) : IGetService<PlaceBetParlayRequest, PlaceBetParlayResult>
{
	public async ValueTask<PlaceBetParlayResult> GetAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new PlaceBetParlayResult("0", [], "");

			_ = await apiLogsRepository.AddAsync(query.message.Action, query, res, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetParlayResult("203", null, "");
		}
	}
}