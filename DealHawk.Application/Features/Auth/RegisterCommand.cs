using MediatR;
using Microsoft.AspNetCore.Identity;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Auth
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; }
        public string Username { get; set; } 
        public string Password { get; set; } 
        public string FullName { get; set; } 
        public string Role { get; set; } = "User";
    }
}
