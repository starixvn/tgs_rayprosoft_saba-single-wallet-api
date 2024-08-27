using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBet3rd;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBet3rdService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<ConfirmBet3rdRequest, ConfirmBet3rdResult>
{
	public async ValueTask<ConfirmBet3rdResult> GetAsync(ConfirmBet3rdRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new ConfirmBet3rdResult("204","success",1000);

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
			return new ConfirmBet3rdResult("",null,0 );
		}
	}
}
