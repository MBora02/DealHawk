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
            var trendingPaged = await _apiClient.GetGamesAsync(pageSize: 4, sortBy: "trending");
            var bestDealsPaged = await _apiClient.GetGamesAsync(pageSize: 4, sortBy: "savings");

            var model = new HomeViewModel
            {
                TrendingGames = trendingPaged.Items,
                BestDeals = bestDealsPaged.Items
            };

            return View(model);
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
