using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class LikeReviewCommand : IRequest<bool>
    {
        public int ReviewId { get; set; }
    }
}
