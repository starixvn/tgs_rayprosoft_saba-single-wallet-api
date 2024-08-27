using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Placebet3rd;
using SabaSingleWallet.Common;
using Tgs.ServiceUtility;
namespace SabaSingleWallet.API.Services;

public class Placebet3rdService(ISapaApiMessagePublishHandler messagePublisher) : IGetService<Placebet3rdRequest, Placebet3rdResult>
{
	public async ValueTask<Placebet3rdResult> GetAsync(Placebet3rdRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new Placebet3rdResult
			{
				Txns = new List<Placebet3rdTicketInfoMapping>
					{
						new Placebet3rdTicketInfoMapping
						{
							RefId = "4200500_284432",
							LicenseeTxId = "1277695"
						}
					},
				Balance = 100.00m,
				Status = "success"
			};

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
			return new Placebet3rdResult
			{
				Txns = new List<Placebet3rdTicketInfoMapping>
					{
						new Placebet3rdTicketInfoMapping
						{
							RefId = "unique-ref-id",
							LicenseeTxId = "merchant-transaction-id"
						}
					},
				Balance = 00m,
				Status = "false"
			};

		}
	}
}