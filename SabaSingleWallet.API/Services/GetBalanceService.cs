using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Common;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class GetBalanceService(ICashServiceClient cashClient, ISapaApiMessagePublishHandler messagePublisher) : IGetService<GetBalanceRequest, GetBalanceResult>
{
	public async ValueTask<GetBalanceResult> GetAsync(GetBalanceRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.Message.UserId.ToPlayerCode();

			var response = await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken).ConfigureAwait(false);

			var result = new GetBalanceResult("0", query.Message.UserId, response.Balance, DateTime.UtcNow.ToString(), response.ErrorMessage);

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.Key,
				Action = query.Message.Action,
				Request = query,
				Response = result
			}, cancellationToken);

			return result;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new GetBalanceResult("203", null, null, null, "Account not existed");
		}
	}
}