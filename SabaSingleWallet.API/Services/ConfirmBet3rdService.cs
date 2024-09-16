using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet3rd;
using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Services;

public class ConfirmBet3rdService(ISapaApiMessagePublishHandler messagePublisher) : IRequestHandler<ConfirmBet3rdRequest, ConfirmBet3rdResult>
{
	public async Task<ConfirmBet3rdResult> Handle(ConfirmBet3rdRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBet3rdResult("204", "success", 1000);

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
			return new ConfirmBet3rdResult("", null, 0);
		}
	}
}