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
    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, IEnumerable<GameDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetWishlistQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<GameDto>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var wishlistedGames = await _unitOfWork.Repository<Wishlist>().Query()
                .Include(w => w.Game).ThenInclude(g => g.Publisher)
                .Include(w => w.Game).ThenInclude(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(w => w.Game).ThenInclude(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
                .Include(w => w.Game).ThenInclude(g => g.StoreGames).ThenInclude(sg => sg.Store)
                .Include(w => w.Game).ThenInclude(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Include(w => w.Game).ThenInclude(g => g.Reviews)
                .Where(w => w.UserId == userId)
                .Select(w => w.Game)
                .ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GameDto>>(wishlistedGames);
        }
    }
}
