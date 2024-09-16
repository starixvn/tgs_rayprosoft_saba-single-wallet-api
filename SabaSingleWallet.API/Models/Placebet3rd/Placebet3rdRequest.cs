using MediatR;

namespace SabaSingleWallet.API.Models.Placebet3rd;

public record Placebet3rdRequest(
   string key,
   Placebet3rdData message
   ) : IRequest<Placebet3rdResult>;