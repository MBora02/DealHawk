using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class GetGameReviewsQueryHandler : IRequestHandler<GetGameReviewsQuery, IEnumerable<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public GetGameReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<ReviewDto>> Handle(GetGameReviewsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var reviews = await _unitOfWork.Repository<Review>().Query()
                .Include(r => r.User)
                .Include(r => r.Game)
                .Include(r => r.ReviewLikes)
                .Where(r => r.GameId == request.GameId && r.Status == ReviewStatus.Approved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<ReviewDto>>(reviews);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                for (int i = 0; i < reviews.Count; i++)
                {
                    dtos[i].IsLikedByCurrentUser = reviews[i].ReviewLikes.Any(l => l.UserId == currentUserId);
                }
            }
            return dtos;
        }
    }
}
