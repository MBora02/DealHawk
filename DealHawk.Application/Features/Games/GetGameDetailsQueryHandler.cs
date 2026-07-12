using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class GetGameDetailsQueryHandler : IRequestHandler<GetGameDetailsQuery, GameDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetGameDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GameDto?> Handle(GetGameDetailsQuery request, CancellationToken cancellationToken)
        {
            var game = await _unitOfWork.Repository<Game>().Query()
                .Include(g => g.Publisher)
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(g => g.GamePlatforms).ThenInclude(gp => gp.Platform)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.Store)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Include(g => g.Reviews).ThenInclude(r => r.User)
                .Include(g => g.Reviews).ThenInclude(r => r.ReviewLikes)
                .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

            if (game == null) return null;
            return _mapper.Map<GameDto>(game);
        }
    }
}
