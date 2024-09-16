using MediatR;

namespace SabaSingleWallet.API.Models.Placebetent;

public record PlacebetentRequest(
   string key,
   PlacebetentData message
   ) : IRequest<PlacebetentResult>;