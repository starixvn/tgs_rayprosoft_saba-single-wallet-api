using System.Text.Json;
using SabaSingleWallet.API.Models.PlaceBet;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services
{
    public class PlaceBetGetService : IGetService<PlaceBetRequest, PlaceBetResult>
    {
        public ValueTask<PlaceBetResult> GetAsync(PlaceBetRequest query, CancellationToken cancellationToken = default)
        {
            try
            {

                Console.WriteLine(query.message);

                //TO DO
                //#1. Check Valid Request
                //#2. Check Valid Bet Amount
                //#3. Check Valid Balance

                return ValueTask.FromResult(new PlaceBetResult("0", "4200500_2331", "1988754", ""));
            }
            catch (Exception ex) {
                return ValueTask.FromResult(new PlaceBetResult("1", "", "", ""));
            }
            
        }
    }
}
