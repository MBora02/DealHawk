using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class UserAdminDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; } 
        public List<string> Roles { get; set; } = new List<string>();
    }
}
