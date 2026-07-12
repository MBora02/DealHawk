using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class GetPriceHistoryQueryHandler : IRequestHandler<GetPriceHistoryQuery, IEnumerable<PriceHistoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPriceHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PriceHistoryDto>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
        {
            var histories = await _unitOfWork.Repository<PriceHistory>().Query()
                .Include(ph => ph.StoreGame).ThenInclude(sg => sg.Store)
                .Where(ph => ph.StoreGame.GameId == request.GameId)
                .OrderBy(ph => ph.RecordedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<PriceHistoryDto>>(histories);
        }
    }
}
