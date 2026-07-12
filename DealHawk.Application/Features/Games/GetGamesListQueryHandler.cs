using AutoMapper;
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
    public class GetGamesListQueryHandler : IRequestHandler<GetGamesListQuery, PaginatedListDto<GameDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        public GetGamesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }
        public async Task<PaginatedListDto<GameDto>> Handle(GetGamesListQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"GamesList_{request.Search}_{request.Genre}_{request.Platform}_{request.PageNumber}_{request.PageSize}_{request.SortBy}";

            if (!request.BypassCache)
            {
                var cachedGames = _cacheService.Get<PaginatedListDto<GameDto>>(cacheKey);
                if (cachedGames != null)
                {
                    return cachedGames;
                }
            }
            var query = _unitOfWork.Repository<Game>().Query()
                .Include(g => g.Publisher)
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.Store)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Include(g => g.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(g => g.Title.ToLower().Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(request.Genre))
            {
                var genreLower = request.Genre.Trim().ToLower();
                query = query.Where(g => g.GameGenres.Any(gg => gg.Genre.Name.ToLower() == genreLower));
            }
            if (!string.IsNullOrWhiteSpace(request.Platform))
            {
                var platformLower = request.Platform.Trim().ToLower();
                query = query.Where(g => g.GamePlatforms.Any(gp => gp.Platform.Name.ToLower() == platformLower));
            }
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                var sortByLower = request.SortBy.Trim().ToLower();
                if (sortByLower == "trending")
                {
                    query = query.OrderByDescending(g => g.Favorites.Count + g.Wishlists.Count);
                }
                else if (sortByLower == "savings")
                {
                    query = query.OrderByDescending(g => g.StoreGames
                        .SelectMany(sg => sg.CurrentPrices)
                        .Max(cp => (decimal?)cp.SavingsPercent) ?? 0);
                }
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var gamesList = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            var mappedGames = _mapper.Map<IEnumerable<GameDto>>(gamesList).ToList();
            var pagedResult = new PaginatedListDto<GameDto>
            {
                Items = mappedGames,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
            _cacheService.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(10));
            return pagedResult;
        }
    }
}
