using Serilog;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.DataAccess;
using Knitted_Toys_Store.App.Mapping;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication;
using Mapster;
using MapsterMapper;
using System.Text.Json.Serialization;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(@"C:\KnittedStoreLogs\log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Используем Serilog для логирования
builder.Host.UseSerilog();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<Knitted_Toys_StoreDBContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(Knitted_Toys_StoreDBContext)));
});

// 1. Сканируем сборку и регистрируем все маппинги из IRegister
TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

// 2. Добавляем Mapster как сервис, если используешь IMapper (необязательно, но удобно)
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

//Регистрация сервисов
builder.Services.AddScoped<IToyService, ToyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

//Регистрация репозиториев
builder.Services.AddScoped<IToysRepositories, ToysRepositories>();
builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();
builder.Services.AddScoped<ICartRepositories, CartRepositories>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(configuration["Redis:Configuration"] ?? "localhost:6379"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000"//,//////////////////////
            //"http://192.168.251.61:3000",
            //"http://109.73.193.165"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services
    .AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var imagesPath = Path.Combine(app.Environment.WebRootPath, "Images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontend");


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCartIdentifier();
app.UseOrderIdentifier();

app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    var env = services.GetRequiredService<IWebHostEnvironment>();

//    if (env.IsDevelopment())
//    {
//        try
//        {
//            var dbContext = services.GetRequiredService<Knitted_Toys_StoreDBContext>();

//            var pendingMigrations = dbContext.Database.GetPendingMigrations();
//            if (pendingMigrations.Any())
//            {
//                logger.LogInformation("Найдены неприменённые миграции. Применяем...");
//                dbContext.Database.Migrate();
//                logger.LogInformation("Миграции успешно применены.");
//            }
//            else
//            {
//                logger.LogInformation("Все миграции уже применены.");
//            }
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Ошибка при применении миграций");
//        }
//    }
//}

app.Run();
