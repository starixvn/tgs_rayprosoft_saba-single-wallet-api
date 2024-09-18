using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.SettleEnt;

namespace SabaSingleWallet.API.Services;

public class SettleEntService(ISapaApiMessagePublishHandler messagePublisher)
	: IRequestHandler<SettleEntRequest, SettleEntResult>
{
	public async Task<SettleEntResult> Handle(SettleEntRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new SettleEntResult("0", "success");

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new SettleEntResult("803", "No Bet Found");
		}
	}
}