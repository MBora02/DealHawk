using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class GetGamesListQuery : IRequest<PaginatedListDto<GameDto>>
    {
        public string? Search { get; set; }
        public string? Genre { get; set; }
        public string? Platform { get; set; }
        public string? SortBy { get; set; }
        public bool BypassCache { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
