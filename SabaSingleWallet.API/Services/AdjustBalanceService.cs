using MediatR;
using SabaSingleWallet.API.Models.AdjustBalance;

namespace SabaSingleWallet.API.Services;

public class AdjustBalanceService() : IRequestHandler<AdjustBalanceRequest, AdjustBalanceResult>
{
	public async Task<AdjustBalanceResult> Handle(AdjustBalanceRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);
			return new AdjustBalanceResult("0", null);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new AdjustBalanceResult("904", msg: "Request Timeout");
		}
	}
}