using System.Collections.Generic;
using DealHawk.Domain.Entities;

namespace DealHawk.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
