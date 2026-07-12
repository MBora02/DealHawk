using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.SqlServer;
using DealHawk.Application.Interfaces;
using DealHawk.Infrastructure.Jobs;
using DealHawk.Infrastructure.Services;
using System;
using System.Text;

namespace DealHawk.Infrastructure
{

    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();

            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddHttpClient<ICheapSharkService, CheapSharkService>(client =>
            {
                client.BaseAddress = new Uri("https://www.cheapshark.com/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "DealHawk/1.0 (contact@dealhawk.com)");
            });

            var jwtKey = configuration["JwtSettings:Key"] ?? "DealHawkSuperSecretJWTKey1234567890!";
            var jwtIssuer = configuration["JwtSettings:Issuer"] ?? "DealHawkAPI";
            var jwtAudience = configuration["JwtSettings:Audience"] ?? "DealHawkWeb";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\mssqllocaldb;Database=DealHawkDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();

            services.AddTransient<PriceSyncJob>();

            return services;
        }
    }
}
