using AutoMapper;
using DealHawk.Application.DTOs;
using DealHawk.Domain.Entities;
using System.Linq;

namespace DealHawk.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurrentPrice, CurrentPriceDto>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreGame.Store.Name))
                .ForMember(dest => dest.StoreLogoUrl, opt => opt.MapFrom(src => src.StoreGame.Store.LogoUrl))
                .ForMember(dest => dest.DeepLinkUrl, opt => opt.MapFrom(src => src.StoreGame.DeepLinkUrl));

            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : null))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.GameGenres.Select(gg => gg.Genre.Name).ToList()))
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src => src.GamePlatforms.Select(gp => gp.Platform.Name).ToList()))
                .ForMember(dest => dest.CurrentPrices, opt => opt.MapFrom(src => src.StoreGames.SelectMany(sg => sg.CurrentPrices).ToList()))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any(r => r.Status == DealHawk.Domain.Enums.ReviewStatus.Approved)
                    ? src.Reviews.Where(r => r.Status == DealHawk.Domain.Enums.ReviewStatus.Approved).Average(r => r.Rating)
                    : 0.0));

            CreateMap<PriceHistory, PriceHistoryDto>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.StoreGame.Store.Name));

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? (string.IsNullOrEmpty(src.User.FullName) ? src.User.UserName : src.User.FullName) : "Unknown"))
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.ReviewLikes.Count))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<PriceAlert, PriceAlertDto>()
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.GameThumbnailUrl, opt => opt.MapFrom(src => src.Game.ThumbnailUrl))
                .ForMember(dest => dest.GameCoverImageUrl, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Game.SteamAppId) && src.Game.SteamAppId != "0" && !string.Equals(src.Game.SteamAppId, "null", StringComparison.OrdinalIgnoreCase)
                    ? $"https://cdn.akamai.steamstatic.com/steam/apps/{src.Game.SteamAppId}/header.jpg"
                    : (string.IsNullOrEmpty(src.Game.ThumbnailUrl)
                        ? "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=400&q=80"
                        : src.Game.ThumbnailUrl)))
                .ForMember(dest => dest.CurrentLowestPrice, opt => opt.MapFrom(src => src.Game.StoreGames
                    .SelectMany(sg => sg.CurrentPrices)
                    .Select(cp => (decimal?)cp.Price)
                    .Min() ?? 0));

            CreateMap<Notification, NotificationDto>();
        }
    }
}
