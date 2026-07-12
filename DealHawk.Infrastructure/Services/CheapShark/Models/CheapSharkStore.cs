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
    internal class CheapSharkStore
    {
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public int IsActive { get; set; }
        public StoreImages Images { get; set; }

    }
}
