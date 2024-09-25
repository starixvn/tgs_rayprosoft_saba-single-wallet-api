using MediatR;
using SabaSingleWallet.API.Models.CancelBetEnt;

namespace SabaSingleWallet.API.Services;

public class CancelBetEntService() : IRequestHandler<CancelBetEntRequest, CancelBetEntResult>
{
	public async Task<CancelBetEntResult> Handle(CancelBetEntRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new CancelBetEntResult("204", "success");

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new CancelBetEntResult("803", "Cancel fail");
		}
	}
}