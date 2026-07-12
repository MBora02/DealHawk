using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.WishlistFavorites
{
    public class ToggleWishlistCommandHandler : IRequestHandler<ToggleWishlistCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ToggleWishlistCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(ToggleWishlistCommand request, CancellationToken cancellationToken)
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
            var existingWish = await _unitOfWork.Repository<Wishlist>().Query()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.GameId == request.GameId, cancellationToken);
            bool isWishlisted;
            if (existingWish != null)
            {
                _unitOfWork.Repository<Wishlist>().Delete(existingWish);
                isWishlisted = false;
            }
            else
            {
                var newWish = new Wishlist
                {
                    UserId = userId,
                    GameId = request.GameId,
                    TargetPrice = request.TargetPrice,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Wishlist>().AddAsync(newWish);
                isWishlisted = true;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return isWishlisted;
        }
    }
}
