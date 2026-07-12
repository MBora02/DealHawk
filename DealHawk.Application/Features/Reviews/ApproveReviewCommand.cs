using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class ApproveReviewCommand : IRequest<bool>
    {
        public int ReviewId { get; set; }
        public string? ModeratorNotes { get; set; }
    }
}
