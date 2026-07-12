using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Reviews
{
    public class SubmitReviewCommand : IRequest<int>
    {
        public int GameId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } 
    }
}
