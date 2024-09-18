using MediatR;
using SabaSingleWallet.API.Models.Placebetent;

namespace SabaSingleWallet.API.Services;

public class PlacebetentService()
	: IRequestHandler<PlacebetentRequest, PlacebetentResult>
{
	public async Task<PlacebetentResult> Handle(PlacebetentRequest query, CancellationToken cancellationToken)
	{
		try
		{
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

			return res;
		}
		catch (Exception ex)
		{
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