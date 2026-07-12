using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace DealHawk.Infrastructure.Jobs
{
    public class PriceSyncJob
    {
        private readonly ICheapSharkService _cheapSharkService;
        private readonly IUnitOfWork _unitOfWork;
        public PriceSyncJob(ICheapSharkService cheapSharkService, IUnitOfWork unitOfWork)
        {
            _cheapSharkService = cheapSharkService;
            _unitOfWork = unitOfWork;
        }
        public async Task RunSyncJobAsync()
        {
            var log = new StoreSyncLog
            {
                JobName = "CheapShark Price & Store Synchronization",
                StartTime = DateTime.UtcNow,
                Status = SyncStatus.Running,
                RecordsProcessed = 0
            };
            var logRepo = _unitOfWork.Repository<StoreSyncLog>();
            await logRepo.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
            try
            {
                await _cheapSharkService.SyncStoresAsync();
                int processed = await _cheapSharkService.SynchronizePricesAsync();
                log.Status = SyncStatus.Success;
                log.EndTime = DateTime.UtcNow;
                log.RecordsProcessed = processed;
                logRepo.Update(log);
            }
            catch (Exception ex)
            {
                log.Status = SyncStatus.Failed;
                log.EndTime = DateTime.UtcNow;
                log.ErrorMessage = ex.Message;
                logRepo.Update(log);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
