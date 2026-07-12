using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ApproveReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
        {
            var reviewerId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(reviewerId))
            {
                throw new UnauthorizedAccessException("Moderator is not authenticated.");
            }
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(request.ReviewId);
            if (review == null) return false;
            review.Status = ReviewStatus.Approved;
            review.ModeratorNotes = request.ModeratorNotes;
            review.ModeratedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Review>().Update(review);
            var auditLog = new AuditLog
            {
                UserId = reviewerId,
                Action = "Review Approved",
                Details = $"Approved review ID {request.ReviewId} with notes: '{request.ModeratorNotes}'.",
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
