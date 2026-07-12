using Microsoft.EntityFrameworkCore;
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
    public class CheapSharkService : ICheapSharkService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.Extensions.Logging.ILogger<CheapSharkService> _logger;
        public CheapSharkService(HttpClient httpClient, IUnitOfWork unitOfWork, Microsoft.Extensions.Logging.ILogger<CheapSharkService> logger)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task SyncStoresAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CheapSharkStore>>("api/1.0/stores");
                if (response == null) return;

                var storeRepo = _unitOfWork.Repository<Store>();
                foreach (var storeData in response)
                {
                    var existingStore = await storeRepo.Query()
                        .FirstOrDefaultAsync(s => s.CheapSharkStoreId == storeData.StoreID);
                    if (existingStore != null)
                    {
                        existingStore.Name = storeData.StoreName;
                        existingStore.IsActive = storeData.IsActive == 1;
                        existingStore.LogoUrl = $"https://www.cheapshark.com{storeData.Images.Logo}";
                        storeRepo.Update(existingStore);
                    }
                    else
                    {
                        var newStore = new Store
                        {
                            Name = storeData.StoreName,
                            CheapSharkStoreId = storeData.StoreID,
                            IsActive = storeData.IsActive == 1,
                            LogoUrl = $"https://www.cheapshark.com{storeData.Images.Logo}"
                        };
                        await storeRepo.AddAsync(newStore);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error syncing stores: {ex.Message}");
            }
        }
        public async Task<int> ImportGamesByTitleAsync(string title)
        {
            int importedCount = 0;
            try
            {

                var searchResults = await _httpClient.GetFromJsonAsync<List<CheapSharkGameSearch>>($"api/1.0/games?title={Uri.EscapeDataString(title)}");
                if (searchResults == null || !searchResults.Any()) return 0;

                var itemsToImport = searchResults.Take(5).ToList();
                var gameRepo = _unitOfWork.Repository<Game>();
                var storeRepo = _unitOfWork.Repository<Store>();
                var publisherRepo = _unitOfWork.Repository<Publisher>();
                var genreRepo = _unitOfWork.Repository<Genre>();
                var platformRepo = _unitOfWork.Repository<Platform>();

                var publishersList = await publisherRepo.Query().ToListAsync();
                var genresList = await genreRepo.Query().ToListAsync();
                var platformsList = await platformRepo.Query().ToListAsync();
                var activeStores = await storeRepo.Query().Where(s => s.IsActive).ToListAsync();

                var random = new Random();
                foreach (var result in itemsToImport)
                {

                    bool isNew = !await gameRepo.Query().AnyAsync(g => g.CheapSharkGameId == result.GameID);

                    bool saved = await SaveGameWithDealsAsync(
                        result.GameID,
                        result.External,
                        result.Thumb,
                        result.SteamAppID,
                        publishersList,
                        genresList,
                        platformsList,
                        activeStores);

                    if (saved && isNew)
                    {
                        importedCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing games by title");
            }

            return importedCount;
        }
        private async Task<bool> SaveGameWithDealsAsync(
            string gameId,
            string title,
            string thumb,
            string steamAppId,
            List<Publisher> publishersList,
            List<Genre> genresList,
            List<Platform> platformsList,
            List<Store> activeStores)
        {
            try
            {
                var gameRepo = _unitOfWork.Repository<Game>();
                var random = new Random();

                var existingGame = await gameRepo.Query()
                    .Include(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                    .FirstOrDefaultAsync(g => g.CheapSharkGameId == gameId || g.Title == title);
                Game gameEntity;
                if (existingGame != null)
                {
                    gameEntity = existingGame;
                    if (!string.IsNullOrEmpty(title)) gameEntity.Title = title;
                    if (!string.IsNullOrEmpty(thumb)) gameEntity.ThumbnailUrl = thumb;
                    if (!string.IsNullOrEmpty(steamAppId)) gameEntity.SteamAppId = steamAppId;
                    gameRepo.Update(gameEntity);
                }
                else
                {
                    gameEntity = new Game
                    {
                        Title = title,
                        CheapSharkGameId = gameId,
                        ThumbnailUrl = thumb ?? "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=400&q=80",
                        SteamAppId = steamAppId,
                        ReleaseDate = DateTime.UtcNow.AddYears(-random.Next(1, 5))
                    };

                    if (publishersList.Any())
                    {
                        gameEntity.PublisherId = publishersList[random.Next(publishersList.Count)].Id;
                    }
                    await gameRepo.AddAsync(gameEntity);
                    await _unitOfWork.SaveChangesAsync();
                    if (genresList.Any())
                    {
                        var mockGenre = genresList[random.Next(genresList.Count)];
                        var gameGenre = new GameGenre { GameId = gameEntity.Id, GenreId = mockGenre.Id };
                        await _unitOfWork.Repository<GameGenre>().AddAsync(gameGenre);
                    }
                    if (platformsList.Any())
                    {
                        var pcPlatform = platformsList.FirstOrDefault(p => p.Name == "PC") ?? platformsList[0];
                        var gamePlatform = new GamePlatform { GameId = gameEntity.Id, PlatformId = pcPlatform.Id };
                        await _unitOfWork.Repository<GamePlatform>().AddAsync(gamePlatform);
                    }
                }

                CheapSharkGameDetail? detail = null;
                try
                {
                    await Task.Delay(300);
                    detail = await _httpClient.GetFromJsonAsync<CheapSharkGameDetail>($"api/1.0/games?id={gameId}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to fetch details from CheapShark for GameId: {GameId} due to rate limits or network issues. Fallback to mock pricing. Error: {Message}", gameId, ex.Message);
                }

                if (detail != null && detail.Deals != null)
                {
                    foreach (var deal in detail.Deals)
                    {
                        var store = activeStores.FirstOrDefault(s => s.CheapSharkStoreId == deal.StoreID);
                        if (store == null) continue;
                        var storeGameRepo = _unitOfWork.Repository<StoreGame>();
                        var storeGame = gameEntity.StoreGames.FirstOrDefault(sg => sg.StoreId == store.Id);
                        decimal price = decimal.TryParse(deal.Price, out var pVal) ? pVal : 0;
                        decimal retail = decimal.TryParse(deal.RetailPrice, out var rVal) ? rVal : 0;
                        decimal savings = decimal.TryParse(deal.Savings, out var sVal) ? sVal : 0;
                        if (storeGame == null)
                        {
                            storeGame = new StoreGame
                            {
                                GameId = gameEntity.Id,
                                StoreId = store.Id,
                                ExternalGameId = gameId,
                                DeepLinkUrl = $"https://www.cheapshark.com/redirect?dealID={deal.DealID}"
                            };
                            await storeGameRepo.AddAsync(storeGame);
                            await _unitOfWork.SaveChangesAsync();
                            gameEntity.StoreGames.Add(storeGame);
                        }
                        else
                        {
                            storeGame.DeepLinkUrl = $"https://www.cheapshark.com/redirect?dealID={deal.DealID}";
                            storeGameRepo.Update(storeGame);
                        }
                        var curPriceRepo = _unitOfWork.Repository<CurrentPrice>();
                        var curPrice = storeGame.CurrentPrices.FirstOrDefault();
                        if (curPrice == null)
                        {
                            curPrice = new CurrentPrice
                            {
                                StoreGameId = storeGame.Id,
                                Price = price,
                                RetailPrice = retail,
                                SavingsPercent = savings,
                                LastUpdated = DateTime.UtcNow
                            };
                            await curPriceRepo.AddAsync(curPrice);
                            storeGame.CurrentPrices.Add(curPrice);
                        }
                        else
                        {
                            curPrice.Price = price;
                            curPrice.RetailPrice = retail;
                            curPrice.SavingsPercent = savings;
                            curPrice.LastUpdated = DateTime.UtcNow;
                            curPriceRepo.Update(curPrice);
                        }
                        var histRepo = _unitOfWork.Repository<PriceHistory>();
                        var lastHistory = await histRepo.Query()
                            .Where(ph => ph.StoreGameId == storeGame.Id)
                            .OrderByDescending(ph => ph.RecordedAt)
                            .FirstOrDefaultAsync();

                        if (lastHistory == null || lastHistory.Price != price)
                        {
                            var historyEntry = new PriceHistory
                            {
                                StoreGameId = storeGame.Id,
                                Price = price,
                                RecordedAt = DateTime.UtcNow
                            };
                            await histRepo.AddAsync(historyEntry);
                        }
                    }
                }
                await GenerateMockDealsIfMissingAsync(gameEntity, activeStores);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SaveGameWithDealsAsync for GameId: {GameId}", gameId);
                return false;
            }
        }
        public async Task<int> SynchronizePricesAsync()
        {
            int updatedCount = 0;
            try
            {
                var games = await _unitOfWork.Repository<Game>().Query()
                    .Include(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                    .Include(g => g.StoreGames).ThenInclude(sg => sg.PriceHistories)
                    .Include(g => g.StoreGames).ThenInclude(sg => sg.Store)
                    .ToListAsync();
                var activeStores = await _unitOfWork.Repository<Store>().Query().Where(s => s.IsActive).ToListAsync();
                foreach (var game in games)
                {
                    if (string.IsNullOrEmpty(game.CheapSharkGameId)) continue;

                    CheapSharkGameDetail? detail = null;
                    try
                    {
                        await Task.Delay(300);
                        detail = await _httpClient.GetFromJsonAsync<CheapSharkGameDetail>($"api/1.0/games?id={game.CheapSharkGameId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to sync details from CheapShark for GameId: {GameId} due to rate limiting or network. Error: {Message}", game.CheapSharkGameId, ex.Message);
                    }

                    if (detail == null || detail.Deals == null)
                    {
                        await GenerateMockDealsIfMissingAsync(game, activeStores);
                        continue;
                    }
                    decimal lowestPrice = decimal.MaxValue;
                    foreach (var deal in detail.Deals)
                    {
                        var store = activeStores.FirstOrDefault(s => s.CheapSharkStoreId == deal.StoreID);
                        if (store == null) continue;
                        var storeGame = game.StoreGames.FirstOrDefault(sg => sg.StoreId == store.Id);
                        decimal price = decimal.TryParse(deal.Price, out var pVal) ? pVal : 0;
                        decimal retail = decimal.TryParse(deal.RetailPrice, out var rVal) ? rVal : 0;
                        decimal savings = decimal.TryParse(deal.Savings, out var sVal) ? sVal : 0;
                        if (price < lowestPrice) lowestPrice = price;
                        if (storeGame == null)
                        {
                            storeGame = new StoreGame
                            {
                                GameId = game.Id,
                                StoreId = store.Id,
                                ExternalGameId = !string.IsNullOrEmpty(game.CheapSharkGameId) ? game.CheapSharkGameId : "mock_" + game.Id,
                                DeepLinkUrl = $"https://www.cheapshark.com/redirect?dealID={deal.DealID}"
                            };
                            game.StoreGames.Add(storeGame);
                        }
                        else
                        {
                            storeGame.DeepLinkUrl = $"https://www.cheapshark.com/redirect?dealID={deal.DealID}";
                        }
                        var curPrice = storeGame.CurrentPrices.FirstOrDefault();
                        if (curPrice == null)
                        {
                            curPrice = new CurrentPrice
                            {
                                Price = price,
                                RetailPrice = retail,
                                SavingsPercent = savings,
                                LastUpdated = DateTime.UtcNow
                            };
                            storeGame.CurrentPrices.Add(curPrice);
                        }
                        else
                        {
                            curPrice.Price = price;
                            curPrice.RetailPrice = retail;
                            curPrice.SavingsPercent = savings;
                            curPrice.LastUpdated = DateTime.UtcNow;
                        }

                        var lastHistory = storeGame.PriceHistories.OrderByDescending(ph => ph.RecordedAt).FirstOrDefault();
                        if (lastHistory == null || lastHistory.Price != price)
                        {
                            var historyEntry = new PriceHistory
                            {
                                Price = price,
                                RecordedAt = DateTime.UtcNow
                            };
                            storeGame.PriceHistories.Add(historyEntry);
                        }
                        updatedCount++;
                    }
                    if (lowestPrice != decimal.MaxValue)
                    {
                        var alerts = await _unitOfWork.Repository<PriceAlert>().Query()
                            .Where(pa => pa.GameId == game.Id && !pa.IsTriggered && pa.TargetPrice >= lowestPrice)
                            .ToListAsync();
                        foreach (var alert in alerts)
                        {
                            alert.IsTriggered = true;
                            _unitOfWork.Repository<PriceAlert>().Update(alert);
                            var notification = new Notification
                            {
                                UserId = alert.UserId,
                                Title = "Price Alert Triggered! 🎮",
                                Message = $"Deal Alert: '{game.Title}' is now available for ${lowestPrice:F2}! This meets your target price of ${alert.TargetPrice:F2}. Check it out now!",
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _unitOfWork.Repository<Notification>().AddAsync(notification);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error synchronizing prices: {ex.Message}");
            }

            return updatedCount;
        }
        public async Task<int> ImportBulkGamesAsync(int targetCount)
        {
            _logger.LogInformation("Starting bulk import of {Count} games from CheapShark...", targetCount);
            int importedCount = 0;
            var random = new Random();
            var gameRepo = _unitOfWork.Repository<Game>();
            try
            {
                var publishersList = await _unitOfWork.Repository<Publisher>().Query().ToListAsync();
                var genresList = await _unitOfWork.Repository<Genre>().Query().ToListAsync();
                var platformsList = await _unitOfWork.Repository<Platform>().Query().ToListAsync();
                var activeStores = await _unitOfWork.Repository<Store>().Query().Where(s => s.IsActive).ToListAsync();

                _logger.LogInformation("Loaded metadata lists: Publishers={PubCount}, Genres={GenCount}, Platforms={PlatCount}, ActiveStores={StoreCount}",
                    publishersList.Count, genresList.Count, platformsList.Count, activeStores.Count);

                if (!publishersList.Any() || !genresList.Any() || !platformsList.Any() || !activeStores.Any())
                {
                    _logger.LogWarning("Cannot execute bulk import because seeding metadata is empty!");
                    return 0;
                }
                int page = 0;
                while (importedCount < targetCount)
                {
                    var requestUrl = $"api/1.0/deals?pageNumber={page}";
                    _logger.LogInformation("Fetching CheapShark deals page {Page} from {Url}", page, _httpClient.BaseAddress + requestUrl);

                    var response = await _httpClient.GetAsync(requestUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("CheapShark API returned non-success status: {StatusCode} for page {Page}", response.StatusCode, page);
                        break;
                    }
                    var deals = await response.Content.ReadFromJsonAsync<List<CheapSharkDealDto>>(
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (deals == null || !deals.Any())
                    {
                        _logger.LogInformation("No more deals returned from CheapShark on page {Page}", page);
                        break;
                    }
                    _logger.LogInformation("Successfully fetched {DealCount} deals on page {Page}", deals.Count, page);
                    foreach (var deal in deals)
                    {
                        if (importedCount >= targetCount)
                        {
                            break;
                        }
                        bool isNew = !await gameRepo.Query().AnyAsync(g => g.CheapSharkGameId == deal.GameID || g.Title == deal.Title);

                        if (!isNew)
                        {
                            continue;
                        }
                        bool saved = await SaveGameWithDealsAsync(
                            deal.GameID,
                            deal.Title,
                            deal.Thumb,
                            deal.SteamAppID,
                            publishersList,
                            genresList,
                            platformsList,
                            activeStores);

                        if (saved)
                        {
                            importedCount++;
                        }
                    }
                    page++;
                    if (page > 25) break;
                }
                _logger.LogInformation("Completed bulk import. Total games imported: {Count}", importedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception encountered during bulk import execution!");
            }
            return importedCount;
        }

        private async Task GenerateMockDealsIfMissingAsync(Game gameEntity, List<Store> activeStores)
        {
            var mainStoreNames = new[] { "Steam", "GOG", "Epic Games Store", "Humble Store" };
            decimal basePrice = 59.99m;
            var existingPrice = gameEntity.StoreGames
                .SelectMany(sg => sg.CurrentPrices)
                .FirstOrDefault();
            if (existingPrice != null && existingPrice.RetailPrice > 0)
            {
                basePrice = existingPrice.RetailPrice;
            }
            else if (existingPrice != null && existingPrice.Price > 0)
            {
                basePrice = existingPrice.Price;
            }
            else
            {
                var random = new Random(gameEntity.Id);
                basePrice = Math.Round((decimal)(19.99 + random.NextDouble() * (59.99 - 19.99)), 2);
            }

            bool changed = false;

            foreach (var storeName in mainStoreNames)
            {
                var store = activeStores.FirstOrDefault(s => s.Name.Equals(storeName, StringComparison.OrdinalIgnoreCase));
                if (store == null) continue;
                var storeGame = gameEntity.StoreGames.FirstOrDefault(sg => sg.StoreId == store.Id);
                if (storeGame == null)
                {
                    storeGame = new StoreGame
                    {
                        GameId = gameEntity.Id,
                        StoreId = store.Id,
                        ExternalGameId = !string.IsNullOrEmpty(gameEntity.CheapSharkGameId) ? gameEntity.CheapSharkGameId : "mock_" + gameEntity.Id,
                        DeepLinkUrl = $"https://www.cheapshark.com/redirect?dealID={(!string.IsNullOrEmpty(gameEntity.CheapSharkGameId) ? gameEntity.CheapSharkGameId : "mock_" + gameEntity.Id)}_mock_{store.CheapSharkStoreId}"
                    };
                    gameEntity.StoreGames.Add(storeGame);
                    changed = true;
                }

                var curPrice = storeGame.CurrentPrices.FirstOrDefault();
                if (curPrice == null)
                {
                    var random = new Random(store.Id + gameEntity.Id);
                    decimal discountPercent = 0;
                    int r = random.Next(4);
                    if (r == 1) discountPercent = 10;
                    else if (r == 2) discountPercent = 25;
                    else if (r == 3) discountPercent = 50;

                    decimal salePrice = Math.Round(basePrice * (1 - discountPercent / 100), 2);

                    curPrice = new CurrentPrice
                    {
                        Price = salePrice,
                        RetailPrice = basePrice,
                        SavingsPercent = discountPercent,
                        LastUpdated = DateTime.UtcNow
                    };
                    storeGame.CurrentPrices.Add(curPrice);

                    var historyEntry = new PriceHistory
                    {
                        Price = salePrice,
                        RecordedAt = DateTime.UtcNow
                    };
                    storeGame.PriceHistories.Add(historyEntry);
                    changed = true;
                }
            }

            if (changed)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
