using AutoMapper;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.API.Models.Unsettle;
using SabaSingleWallet.Repositories.Models.GameBet;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.Settle;
using SabaSingleWallet.Repositories.Models.Unsettle;

namespace SabaSingleWallet.API;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<PlaceBetData, PlaceBetCriteria>();
		CreateMap<SettlementTransaction, SettleCriteria>()
			.ForMember(dest => dest.WinlostDate, opt => opt.MapFrom(src => DateTimeOffset.Parse(src.WinlostDate).ToUniversalTime()))
			.ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => DateTimeOffset.Parse(src.UpdateTime).ToUniversalTime()));
		CreateMap<UnsettleTransaction, UnsettleCriteria>()
			.ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => DateTimeOffset.Parse(src.UpdateTime).ToUniversalTime()));	   	
	}

}
