using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Common;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class GetBalanceService(ICashServiceClient cashClient, ISapaApiMessagePublishHandler messagePublisher) : IGetService<GetBalanceRequest, GetBalanceResult>
{
	private const int PlayerCodeIndex = 1;

	public async ValueTask<GetBalanceResult> GetAsync(GetBalanceRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.Message.UserId.Replace("_test", string.Empty).Remove(0, query.Message.UserId.IndexOf("_") + 1);

			var balance = await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken).ConfigureAwait(false);
			var res = new GetBalanceResult("0", query.Message.UserId, balance, DateTime.UtcNow.ToString());

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.Key,
				Action = query.Message.Action,
				Request = query,
				Response = res
			}, cancellationToken);

			return res;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new GetBalanceResult("203", null, null, null, "Account not existed");
		}
	}
}