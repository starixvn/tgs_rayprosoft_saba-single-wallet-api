using AutoMapper;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.Settle;
using SabaSingleWallet.Repositories.Models.Unsettle;

namespace SabaSingleWallet.API;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<PlaceBetData, PlaceBetCriteria>();
		CreateMap<SettlementTransaction, SettleCriteria>();
		CreateMap<UnsettleTransaction, UnsettleCriteria>()
			.ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => DateTimeOffset.Parse(src.UpdateTime).ToUniversalTime()));	   	
	}

}
