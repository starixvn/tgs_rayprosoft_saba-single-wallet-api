using MediatR;

namespace SabaSingleWallet.API.Models.Settle;

public class SettleRequest : IRequest<SettleResult>
{
	public string key { get; set; }

	public SettleData message { get; set; }
}