using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.WishlistFavorites
{
    public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ToggleFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var game = await _unitOfWork.Repository<Game>().GetByIdAsync(request.GameId);
            if (game == null)
            {
                throw new ArgumentException("Game does not exist.");
            }
            var existingFav = await _unitOfWork.Repository<Favorite>().Query()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.GameId == request.GameId, cancellationToken);
            bool isFavorite;
            if (existingFav != null)
            {
                _unitOfWork.Repository<Favorite>().Delete(existingFav);
                isFavorite = false;
            }
            else
            {
                var newFav = new Favorite
                {
                    UserId = userId,
                    GameId = request.GameId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Favorite>().AddAsync(newFav);
                isFavorite = true;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return isFavorite;
        }
    }
}
