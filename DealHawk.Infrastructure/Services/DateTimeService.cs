using DealHawk.Application.Interfaces;
using System;

namespace DealHawk.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
