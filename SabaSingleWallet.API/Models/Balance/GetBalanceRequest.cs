using MediatR;
using SabaSingleWallet.API.Services._Shared;

namespace SabaSingleWallet.API.Models.Balance;

[IgnoreLogging]
public class GetBalanceRequest : IRequest<GetBalanceResult>
{
	public string Key { get; set; }
	public GetBalanceData Message { get; set; }
}