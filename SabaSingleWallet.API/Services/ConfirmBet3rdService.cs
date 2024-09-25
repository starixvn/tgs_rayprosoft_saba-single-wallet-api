using MediatR;
using SabaSingleWallet.API.Models.ConfirmBet3rd;

namespace SabaSingleWallet.API.Services;

public class ConfirmBet3rdService() : IRequestHandler<ConfirmBet3rdRequest, ConfirmBet3rdResult>
{
	public async Task<ConfirmBet3rdResult> Handle(ConfirmBet3rdRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBet3rdResult("204", "success", 1000);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new ConfirmBet3rdResult("", null, 0);
		}
	}
}