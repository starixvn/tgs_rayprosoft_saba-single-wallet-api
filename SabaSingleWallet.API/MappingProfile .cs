using AutoMapper;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.PlaceBet;

namespace SabaSingleWallet.API;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<PlaceBetData, PlaceBetCriteria>();
	}
}
