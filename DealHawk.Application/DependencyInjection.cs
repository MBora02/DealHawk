using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AutoMapper;
using DealHawk.Application.Mapping;
using System.Reflection;

namespace DealHawk.Application
{

    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
