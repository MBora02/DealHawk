using MediatR;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Dashboard
{
    public class GetAdminDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
    }
}
