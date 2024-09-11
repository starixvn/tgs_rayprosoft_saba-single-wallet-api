namespace SabaSingleWallet.Repositories;
using AutoMapper;
using Fanex.Data.Repository;

public interface IFanexPlayerRepository
{
	Task<TResult> CallStoredProcedureAsync<TResult>(ICriteria criteria);
};

public class FanexPlayerRepositoryImp : IFanexPlayerRepository
{
	private readonly IDynamicRepository dynamicRepository;

	public FanexPlayerRepositoryImp(IDynamicRepository dynamicRepository, IMapper mapper)
	{
		this.dynamicRepository = dynamicRepository;

	}

	public async Task<TResult> CallStoredProcedureAsync<TResult>(ICriteria criteria)
		=> await dynamicRepository.GetAsync<TResult>(criteria);
}
