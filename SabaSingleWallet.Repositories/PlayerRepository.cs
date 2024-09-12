namespace SabaSingleWallet.Repositories;
using AutoMapper;
using Fanex.Data.Repository;

public interface IPlayerRepository
{
	Task<TResult> CallStoredProcedureAsync<TResult>(ICriteria criteria);
};

public class PlayerRepositoryImp : IPlayerRepository
{
	private readonly IDynamicRepository dynamicRepository;

	public PlayerRepositoryImp(IDynamicRepository dynamicRepository, IMapper mapper)
	{
		this.dynamicRepository = dynamicRepository;

	}

	public async Task<TResult> CallStoredProcedureAsync<TResult>(ICriteria criteria)
		=> await dynamicRepository.GetAsync<TResult>(criteria);
}
