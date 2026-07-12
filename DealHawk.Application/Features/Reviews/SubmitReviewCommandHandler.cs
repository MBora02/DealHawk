using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public SubmitReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<int> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
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
            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }
            var review = new Review
            {
                UserId = userId,
                GameId = request.GameId,
                Rating = request.Rating,
                Comment = request.Comment,
                Status = ReviewStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Review>().AddAsync(review);
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "Review Submitted",
                Details = $"Submitted pending review for game ID {request.GameId} with rating {request.Rating}.",
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return review.Id;
        }
    }
}
