using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Hangfire;
using Serilog;
using Scalar.AspNetCore;
using DealHawk.API.Middlewares;
using DealHawk.API.Services;
using DealHawk.Application;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Infrastructure;
using DealHawk.Infrastructure.Jobs;
using DealHawk.Persistence;
using DealHawk.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/dealhawk_api_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "DealHawk REST API",
            Version = "v1",
            Description = "DealHawk Game Price Tracker & Deal Monitoring Platform API"
        };

        document.Components ??= new();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Input your JWT token in the format: Bearer {token}"
        });

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new()
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        await DatabaseSeeder.SeedDataAsync(scope.ServiceProvider);
        Log.Information("Database successfully seeded.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during database seeding.");
    }
}

_ = Task.Run(async () =>
{
    try
    {
        using var backgroundScope = app.Services.CreateScope();
        var cheapSharkService = backgroundScope.ServiceProvider.GetRequiredService<ICheapSharkService>();
        await cheapSharkService.SyncStoresAsync();
        Log.Information("Active stores list successfully synchronized from CheapShark in background.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during background store synchronization.");
    }
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DealHawk API Playground")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = Array.Empty<Hangfire.Dashboard.IDashboardAuthorizationFilter>()
});

RecurringJob.AddOrUpdate<PriceSyncJob>(
    "CheapSharkSync",
    job => job.RunSyncJobAsync(),
    "0 */6 * * *");

app.MapControllers();

app.Run();
