using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.PriceAlerts
{
    public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CreateAlertCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<int> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
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
            var lowestPrice = await _unitOfWork.Repository<StoreGame>().Query()
                .Where(sg => sg.GameId == request.GameId)
                .SelectMany(sg => sg.CurrentPrices)
                .Select(cp => (decimal?)cp.Price)
                .MinAsync(cancellationToken);
            bool shouldTrigger = lowestPrice.HasValue && lowestPrice.Value <= request.TargetPrice;
            var existingAlert = await _unitOfWork.Repository<PriceAlert>().Query()
                .FirstOrDefaultAsync(pa => pa.UserId == userId && pa.GameId == request.GameId, cancellationToken);
            if (existingAlert != null)
            {
                existingAlert.TargetPrice = request.TargetPrice;
                existingAlert.IsTriggered = shouldTrigger;
                _unitOfWork.Repository<PriceAlert>().Update(existingAlert);
            }
            else
            {
                var newAlert = new PriceAlert
                {
                    UserId = userId,
                    GameId = request.GameId,
                    TargetPrice = request.TargetPrice,
                    IsTriggered = shouldTrigger,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<PriceAlert>().AddAsync(newAlert);
            }
            if (shouldTrigger)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = "Price Alert Triggered! 🎮",
                    Message = $"Deal Alert: '{game.Title}' is currently available for ${lowestPrice!.Value:F2}! This meets your target price of ${request.TargetPrice:F2}. Check it out now!",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Notification>().AddAsync(notification);
            }
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "Price Alert Created/Updated",
                Details = $"Created price alert for game ID {request.GameId} at target price {request.TargetPrice}. Triggered immediately: {shouldTrigger}.",
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return request.GameId;
        }
    }
}
