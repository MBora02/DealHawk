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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
        }
        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Email already registered." };
            }
            existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Username is already taken." };
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username,
                FullName = request.FullName,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = string.Join(", ", result.Errors) };
            }
            var roleName = request.Role;
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);
            var roles = new List<string> { roleName };
            var token = _tokenGenerator.GenerateToken(user, roles);
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Registration successful.",
                Token = token,
                Username = user.UserName!,
                Email = user.Email!,
                Roles = roles,
                Expiration = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
