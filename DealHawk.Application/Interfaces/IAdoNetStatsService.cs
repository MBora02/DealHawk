using System.Threading.Tasks;
using DealHawk.Application.DTOs;

namespace DealHawk.Application.Interfaces
{
    public interface IAdoNetStatsService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
