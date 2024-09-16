using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBetEnt;
using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Services;

public class CancelBetEntService(ISapaApiMessagePublishHandler messagePublisher) : IRequestHandler<CancelBetEntRequest, CancelBetEntResult>
{
	public async Task<CancelBetEntResult> Handle(CancelBetEntRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new CancelBetEntResult("204", "success");

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new CancelBetEntResult("803", "Cancel fail");
		}
	}
}