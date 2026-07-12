using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class ImportGamesCommandHandler : IRequestHandler<ImportGamesCommand, int>
    {
        private readonly ICheapSharkService _cheapSharkService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ImportGamesCommandHandler(
            ICheapSharkService cheapSharkService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _cheapSharkService = cheapSharkService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<int> Handle(ImportGamesCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title)) return 0;
            var importedCount = await _cheapSharkService.ImportGamesByTitleAsync(request.Title);
            var auditLog = new AuditLog
            {
                UserId = _currentUserService.UserId,
                Action = "Game Import Triggered",
                Details = $"Imported {importedCount} games from CheapShark for search term '{request.Title}'.",
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.Repository<AuditLog>().AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return importedCount;
        }
    }
}
