using MediatR;

namespace SabaSingleWallet.API.Models.GetTicketInfo;

public record GetTicketInfoRequest(
   string key,
   GetTicketInfoData message
   ) : IRequest<GetTicketInfoResult>;