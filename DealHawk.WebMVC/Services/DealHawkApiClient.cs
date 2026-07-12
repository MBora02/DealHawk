using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DealHawk.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Services
{

    public class DealHawkApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DealHawkApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5233/";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirstValue("Token");
            Console.WriteLine($"[DealHawkApiClient] Attaching Token: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : token.Substring(0, 15) + "...")}");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<AuthResponseDto?> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            var err = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return err ?? new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
        }

        public async Task<AuthResponseDto?> RegisterAsync(string email, string username, string password, string fullName)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { email, username, password, fullName });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            var err = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return err ?? new AuthResponseDto { IsSuccess = false, Message = "Registration failed." };
        }

        public async Task<PaginatedListDto<GameDto>> GetGamesAsync(string? search = null, string? genre = null, string? platform = null, int pageNumber = 1, int pageSize = 20, string? sortBy = null)
        {
            AddAuthHeader();
            var url = $"api/games?search={Uri.EscapeDataString(search ?? "")}&genre={Uri.EscapeDataString(genre ?? "")}&platform={Uri.EscapeDataString(platform ?? "")}&pageNumber={pageNumber}&pageSize={pageSize}&sortBy={Uri.EscapeDataString(sortBy ?? "")}&bypassCache=true";
            return await _httpClient.GetFromJsonAsync<PaginatedListDto<GameDto>>(url) ?? new PaginatedListDto<GameDto>();
        }

        public async Task<GameDto?> GetGameByIdAsync(int id)
        {
            AddAuthHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<GameDto>($"api/games/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PriceHistoryDto>> GetPriceHistoryAsync(int gameId)
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<PriceHistoryDto>>($"api/games/{gameId}/history") ?? new List<PriceHistoryDto>();
        }

        public async Task<int> ImportGamesAsync(string title)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/games/import", new { title });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<List<GameDto>> GetWishlistAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<GameDto>>("api/wishlist") ?? new List<GameDto>();
        }

        public async Task<bool> ToggleWishlistAsync(int gameId, decimal? targetPrice = null)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/wishlist/toggle", new { gameId, targetPrice });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<GameDto>> GetFavoritesAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<GameDto>>("api/favorites") ?? new List<GameDto>();
        }

        public async Task<bool> ToggleFavoriteAsync(int gameId)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/favorites/toggle", new { gameId });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<PriceAlertDto>> GetUserAlertsAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<PriceAlertDto>>("api/alerts") ?? new List<PriceAlertDto>();
        }

        public async Task<int> CreateAlertAsync(int gameId, decimal targetPrice)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/alerts", new { gameId, targetPrice });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<bool> DeleteAlertAsync(int alertId)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/alerts/{alertId}");
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(bool onlyUnread = false)
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/notifications?onlyUnread={onlyUnread}") ?? new List<NotificationDto>();
        }

        public async Task<bool> MarkNotificationsReadAsync(int? notificationId = null)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/notifications/mark-read", new { notificationId });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<ReviewDto>> GetGameReviewsAsync(int gameId)
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<ReviewDto>>($"api/reviews/game/{gameId}") ?? new List<ReviewDto>();
        }

        public async Task<int> SubmitReviewAsync(int gameId, int rating, string comment)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/reviews", new { gameId, rating, comment });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<bool> LikeReviewAsync(int reviewId)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"api/reviews/{reviewId}/like", new { });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<ReviewDto>> GetPendingReviewsAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<ReviewDto>>("api/reviews/pending") ?? new List<ReviewDto>();
        }

        public async Task<bool> ApproveReviewAsync(int reviewId, string? notes)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"api/reviews/{reviewId}/approve", new { moderatorNotes = notes });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> RejectReviewAsync(int reviewId, string? notes)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"api/reviews/{reviewId}/reject", new { moderatorNotes = notes });
            return response.IsSuccessStatusCode && await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
        {
            AddAuthHeader();
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardStatsDto>("api/admin/dashboard-stats");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> TriggerSyncAsync()
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsync("api/admin/sync", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<dynamic>> GetAuditLogsAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<dynamic>>("api/admin/audit-logs") ?? new List<dynamic>();
        }

        public async Task<List<dynamic>> GetSyncLogsAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<dynamic>>("api/admin/sync-logs") ?? new List<dynamic>();
        }

        public async Task<bool> ImportBulkGamesAsync(int count)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsync($"api/admin/import-bulk?count={count}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<UserAdminDto>> GetUsersAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<UserAdminDto>>("api/admin/users") ?? new List<UserAdminDto>();
        }

        public async Task<List<GenreAdminDto>> GetSystemGenresAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<GenreAdminDto>>("api/admin/genres") ?? new List<GenreAdminDto>();
        }

        public async Task<List<StoreAdminDto>> GetSystemStoresAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<StoreAdminDto>>("api/admin/stores") ?? new List<StoreAdminDto>();
        }

        public async Task<List<FavoriteAdminDto>> GetSystemFavoritesAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<FavoriteAdminDto>>("api/admin/favorites") ?? new List<FavoriteAdminDto>();
        }

        public async Task<List<ReviewAdminDto>> GetSystemReviewsAsync()
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<ReviewAdminDto>>("api/admin/reviews") ?? new List<ReviewAdminDto>();
        }

        public async Task<bool> DeleteGameAsync(int gameId)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/admin/games/{gameId}");
            return response.IsSuccessStatusCode;
        }
    }
}
