using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.PriceAlerts
{
    public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public DeleteAlertCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var alert = await _unitOfWork.Repository<PriceAlert>().Query()
                .FirstOrDefaultAsync(pa => pa.Id == request.AlertId && pa.UserId == userId, cancellationToken);
            if (alert == null) return false;
            _unitOfWork.Repository<PriceAlert>().Delete(alert);
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = "Price Alert Deleted",
                Details = $"Deleted price alert ID {request.AlertId} for game ID {alert.GameId}.",
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
