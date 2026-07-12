using Microsoft.Extensions.Logging;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DealHawk.Infrastructure.Services
{
    internal class GameInfo
    {
        public string Title { get; set; }
        public string? SteamAppID { get; set; }
        public string Thumb { get; set; }

    }
}
