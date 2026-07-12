using MediatR;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class ImportGamesCommand : IRequest<int>
    {
        public string Title { get; set; }
    }
}
