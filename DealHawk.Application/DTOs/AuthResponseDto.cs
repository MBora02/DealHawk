using System;
using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime Expiration { get; set; }
    }
}
