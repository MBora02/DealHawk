using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Reviews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class ApproveReviewBody
    {
        public string? ModeratorNotes { get; set; }
    }
}
