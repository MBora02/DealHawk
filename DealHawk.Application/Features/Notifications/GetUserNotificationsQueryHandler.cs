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

namespace DealHawk.Application.Features.Notifications
{
    public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IEnumerable<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetUserNotificationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var query = _unitOfWork.Repository<Notification>().Query()
                .Where(n => n.UserId == userId);
            if (request.OnlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }
    }
}
