using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Common;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
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

			var result = new GetBalanceResult(SabaStatusCodeConst.SuccessCode, query.Message.UserId, response.Balance, DateTime.UtcNow.ToString());

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
			SentrySdk.CaptureException(ex);
			Console.Write(ex.ToString());
			return new GetBalanceResult(SabaStatusCodeConst.ApiInternalErrorCode, null, null, null);
		}
	}
}