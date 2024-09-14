using MediatR;

namespace SabaSingleWallet.API.Models.ConfirmBet;

public class ConfirmBetRequest : IRequest<ConfirmBetResult>
{
	public string key { get; set; }
	public ConfirmBetData message { get; set; }
}