using MediatR;

namespace SabaSingleWallet.API.Models.Balance;

public class GetBalanceRequest : IRequest<GetBalanceResult>
{
	public string Key { get; set; }
	public GetBalanceData Message { get; set; }
}