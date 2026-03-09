
using IIS_portal_backend.DataAccess;
using IIS_portal_backend.DataAccess.IIS_portal_backend.Data;
using IIS_portal_backend.HostedService;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortalConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379")
);

builder.Services.AddHostedService<PortalBackgroundWorker>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();


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
    app.MapOpenApi();
}

app.UseCors(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();