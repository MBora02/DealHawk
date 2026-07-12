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
    internal class CheapSharkDealDto
    {
        public string Title { get; set; }
        public string GameID { get; set; }
        public string SalePrice { get; set; }
        public string NormalPrice { get; set; }
        public string Savings { get; set; }
        public string SteamAppID { get; set; }
        public string Thumb { get; set; }
        public string StoreID { get; set; }
        public string DealID { get; set; }

    }
}
