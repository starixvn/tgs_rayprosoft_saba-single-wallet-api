using MediatR;
using SabaSingleWallet.API.Models.Placebet3rd;

namespace SabaSingleWallet.API.Services;

public class Placebet3rdService()
	: IRequestHandler<Placebet3rdRequest, Placebet3rdResult>
{
	public async Task<Placebet3rdResult> Handle(Placebet3rdRequest query, CancellationToken cancellationToken)
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