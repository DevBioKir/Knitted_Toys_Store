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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AppMappingProfile)); //регистрация AppMappingProfile

builder.Services.AddDbContext<Knitted_Toys_StoreDBContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(Knitted_Toys_StoreDBContext)));
});

//Регистрация сервисов
builder.Services.AddScoped<IToyService, ToyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

//Регистрация репозиториев
builder.Services.AddScoped<IToysRepositories, ToysRepositories>();
builder.Services.AddScoped<IOrderRepositories, OrderRepositories>();
builder.Services.AddScoped<ICartRepositories, CartRepositories>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://192.168.251.61:3000"
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

app.MapControllers();

app.Run();
