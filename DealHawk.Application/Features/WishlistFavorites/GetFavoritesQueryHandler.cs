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

namespace DealHawk.Application.Features.WishlistFavorites
{
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, IEnumerable<GameDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetFavoritesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<GameDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var favoriteGames = await _unitOfWork.Repository<Favorite>().Query()
                .Include(f => f.Game).ThenInclude(g => g.Publisher)
                .Include(f => f.Game).ThenInclude(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(f => f.Game).ThenInclude(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
                .Include(f => f.Game).ThenInclude(g => g.StoreGames).ThenInclude(sg => sg.Store)
                .Include(f => f.Game).ThenInclude(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Include(f => f.Game).ThenInclude(g => g.Reviews)
                .Where(f => f.UserId == userId)
                .Select(f => f.Game)
                .ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GameDto>>(favoriteGames);
        }
    }
}
