using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.ViewComponents
{

    public class NotificationNavViewComponent : ViewComponent
    {
        private readonly DealHawkApiClient _apiClient;

        public NotificationNavViewComponent(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Content(string.Empty);
            }

            try
            {
                var notifications = await _apiClient.GetNotificationsAsync();
                var unreadCount = notifications.Count(n => !n.IsRead);
                return View(unreadCount);
            }
            catch (Exception)
            {
                return View(0);
            }
        }
    }
}
