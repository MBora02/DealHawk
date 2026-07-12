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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.Username);
            }
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
            }
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenGenerator.GenerateToken(user, roles);
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = new List<string>(roles),
                Expiration = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
