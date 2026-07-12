using System.Threading.Tasks;

namespace DealHawk.Application.Interfaces
{
    public interface ICheapSharkService
    {
        Task SyncStoresAsync();
        Task<int> ImportGamesByTitleAsync(string title);
        Task<int> SynchronizePricesAsync();
        Task<int> ImportBulkGamesAsync(int targetCount);
    }
}
