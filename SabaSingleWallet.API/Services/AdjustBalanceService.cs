using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.AdjustBalance;
using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Services;

public class AdjustBalanceService(ISapaApiMessagePublishHandler messagePublisher) : IRequestHandler<AdjustBalanceRequest, AdjustBalanceResult>
{
	public async Task<AdjustBalanceResult> Handle(AdjustBalanceRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new AdjustBalanceResult("0", null);
			return await GenerateResultAsync(query, res);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return await GenerateResultAsync(query, new AdjustBalanceResult("904", msg: "Request Timeout"));
		}
	}

	private async Task<AdjustBalanceResult> GenerateResultAsync(AdjustBalanceRequest query, AdjustBalanceResult result)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = query.key,
			Action = query.message.Action,
			Request = query,
			Response = result
		});

		return result;
	}
}