
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.CancelBetEnt;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class CancelBetEntService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<CancelBetEntRequest, CancelBetEntResult>
{
	public async ValueTask<CancelBetEntResult> GetAsync(CancelBetEntRequest query, CancellationToken cancellationToken = default)
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