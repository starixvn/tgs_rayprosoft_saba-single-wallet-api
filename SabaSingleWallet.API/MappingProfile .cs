using AutoMapper;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using SabaSingleWallet.Repositories.Models.Settle;

namespace SabaSingleWallet.API;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<PlaceBetData, PlaceBetCriteria>();
		CreateMap<SettlementTransaction, SettleCriteria>();
	}
}
