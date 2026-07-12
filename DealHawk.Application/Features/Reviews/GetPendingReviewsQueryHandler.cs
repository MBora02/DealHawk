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
    public class GetPendingReviewsQueryHandler : IRequestHandler<GetPendingReviewsQuery, IEnumerable<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPendingReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ReviewDto>> Handle(GetPendingReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.Repository<Review>().Query()
                .Include(r => r.User)
                .Include(r => r.Game)
                .Include(r => r.ReviewLikes)
                .Where(r => r.Status == ReviewStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
    }
}
