using IIS_backend.Caching;
using IIS_backend.DataAccess;
using IIS_backend.HostedService;
using IIS_backend.Services;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName); 
});

// configure exchange rate api settings
builder.Services.Configure<ExchangeRateApiSettings>(
    builder.Configuration.GetSection(ExchangeRateApiSettings.SectionName)
);

// configure serilog
builder.Services.AddSerilog((services, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration)
);

// configure EF Core (PostgreSQL)
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// configure exchange rate http client


//**************************************-------------------------------------------*********************************************************
// -------------------------------------------------------------------------------------
builder.Services.AddHttpClient<IExchangeRateClient, ExchangeRateClient>();

// naši servisi
builder.Services.AddTransient<ICompetitionService, CompetitionService>();
builder.Services.AddTransient<IDayService, DayService>();
builder.Services.AddTransient<IZoneService, ZoneService>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<ITicketService, TicketService>();

// background services (queue subscriber za kreiranje karte)
builder.Services.AddHostedService<BackgroundWorker>();

// add redis (cache + pub/sub queue)
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379")
);
builder.Services.AddTransient<ICacheService, RedisCacheService>();

// add cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        if (exceptionHandlerPathFeature?.Error is Exception ex)
        {
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    });
});

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();