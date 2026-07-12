using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Games
{
    public class GetGameDetailsQuery : IRequest<GameDto?>
    {
        public int Id { get; set; }
    }
}
