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
    internal class CheapSharkGameSearch
    {
        public string GameID { get; set; }
        public string SteamAppID { get; set; }
        public string Cheapest { get; set; }
        public string CheapestDealID { get; set; }
        public string External { get; set; }
        public string Thumb { get; set; }

    }
}
