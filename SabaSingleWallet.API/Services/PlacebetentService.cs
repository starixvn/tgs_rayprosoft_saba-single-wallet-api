using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Placebetent;
using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Services;

public class PlacebetentService(ISapaApiMessagePublishHandler messagePublisher)
	: IRequestHandler<PlacebetentRequest, PlacebetentResult>
{
	public async Task<PlacebetentResult> Handle(PlacebetentRequest query, CancellationToken cancellationToken)
	{
		try
		{
			Console.WriteLine(query.message);
			var res = new PlacebetentResult
			{
				Status = "204",
				Msg = "success",
				Balance = 1000.0m,
				UserId = "player",
				TicketList = new List<PlacebetentTicketDetailResponse>
					{
						new PlacebetentTicketDetailResponse
						{
							RefId = "4200500_63",
							LicenseeTxId = "5800"
						}
					}
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
			return new PlacebetentResult
			{
				Status = "555",
				Msg = "false",
				Balance = 0.0m,
				UserId = "player",
				TicketList = new List<PlacebetentTicketDetailResponse>
					{
						new PlacebetentTicketDetailResponse
						{
							RefId = "0000032",
							LicenseeTxId = "000"
						}
					}
			};
		}
	}
}