namespace SabaSingleWallet.Core;

public interface IPlayerRepository
{
	Task<TResult> CallStoredProcedureAsync<TCriteria, TResult>(TCriteria criteria, string spName);
}