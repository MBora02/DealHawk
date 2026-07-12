using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class LikeReviewCommandHandler : IRequestHandler<LikeReviewCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public LikeReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(LikeReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(request.ReviewId);
            if (review == null)
            {
                throw new ArgumentException("Review does not exist.");
            }
            var existingLike = await _unitOfWork.Repository<ReviewLike>().Query()
                .FirstOrDefaultAsync(rl => rl.UserId == userId && rl.ReviewId == request.ReviewId, cancellationToken);
            bool isLiked;
            if (existingLike != null)
            {
                _unitOfWork.Repository<ReviewLike>().Delete(existingLike);
                isLiked = false;
            }
            else
            {
                var newLike = new ReviewLike
                {
                    UserId = userId,
                    ReviewId = request.ReviewId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<ReviewLike>().AddAsync(newLike);
                isLiked = true;
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return isLiked;
        }
    }
}
