using System.Text.Json;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Repositories;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services
{
    public class PlaceBetGetService(IPlaceBetLogsRepository placeBetLogsRepository) : IGetService<PlaceBetRequest, PlaceBetResult>
    {
        public async ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
        {
            try
            {

                Console.WriteLine(query.message);
				var res = new PlaceBetResult("0", "4200500_2331", "1988754", "");

				await placeBetLogsRepository.AddAsync(query, res);

				//TO DO
				//#1. Check Valid Request
				//#2. Check Valid Bet Amount
				//#3. Check Valid Balance

				return res;
            }
            catch (Exception ex) {
                return new PlaceBetResult("1", "", "", "");
            }
            
        }
    }
}
