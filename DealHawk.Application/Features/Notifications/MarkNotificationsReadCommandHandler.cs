using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Notifications
{
    public class MarkNotificationsReadCommandHandler : IRequestHandler<MarkNotificationsReadCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public MarkNotificationsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(MarkNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            if (request.NotificationId.HasValue)
            {
                var notification = await _unitOfWork.Repository<Notification>().Query()
                    .FirstOrDefaultAsync(n => n.Id == request.NotificationId.Value && n.UserId == userId, cancellationToken);
                if (notification == null) return false;

                notification.IsRead = true;
                _unitOfWork.Repository<Notification>().Update(notification);
            }
            else
            {
                var unreadNotifications = await _unitOfWork.Repository<Notification>().Query()
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync(cancellationToken);

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    _unitOfWork.Repository<Notification>().Update(notification);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
