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

namespace DealHawk.Application.Features.PriceAlerts
{
    public class GetUserAlertsQueryHandler : IRequestHandler<GetUserAlertsQuery, IEnumerable<PriceAlertDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetUserAlertsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<PriceAlertDto>> Handle(GetUserAlertsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var alerts = await _unitOfWork.Repository<PriceAlert>().Query()
                .Include(pa => pa.Game).ThenInclude(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Where(pa => pa.UserId == userId)
                .ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PriceAlertDto>>(alerts);
        }
    }
}
