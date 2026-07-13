using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Models;
using DealHawk.WebMVC.Services;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    public class HomeController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public HomeController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var trendingPaged = await _apiClient.GetGamesAsync(pageSize: 4, sortBy: "trending");
                var bestDealsPaged = await _apiClient.GetGamesAsync(pageSize: 4, sortBy: "savings");

                var model = new HomeViewModel
                {
                    TrendingGames = trendingPaged.Items,
                    BestDeals = bestDealsPaged.Items
                };

                return View(model);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                ViewBag.BackendError = "The backend API service is currently starting up or offline. Please wait a few seconds and refresh the page.";
                var model = new HomeViewModel
                {
                    TrendingGames = new System.Collections.Generic.List<DealHawk.Application.DTOs.GameDto>(),
                    BestDeals = new System.Collections.Generic.List<DealHawk.Application.DTOs.GameDto>()
                };
                return View(model);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
