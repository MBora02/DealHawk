using MediatR;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Dashboard
{
    public class GetAdminDashboardStatsQueryHandler : IRequestHandler<GetAdminDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IAdoNetStatsService _adoNetStatsService;
        public GetAdminDashboardStatsQueryHandler(IAdoNetStatsService adoNetStatsService)
        {
            _adoNetStatsService = adoNetStatsService;
        }
        public async Task<DashboardStatsDto> Handle(GetAdminDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            return await _adoNetStatsService.GetDashboardStatsAsync();
        }
    }
}
