using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.DataAccess;
using Knitted_Toys_Store.App.Mapping;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Knitted_Toys_Store.API.Middleware;
using Microsoft.AspNetCore.Http.Features;

using System.Diagnostics;

AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    var exception = args.ExceptionObject as Exception;
    Debug.WriteLine($"CRITICAL ERRROR: {exception?.Message}");
    Debug.WriteLine($"Stack Trace: {exception?.StackTrace}");

    File.WriteAllText("crash_log.txt",
        $"Time: {DateTime.Now}\n" +
        $"Message: {exception?.Message}\n + " +
        $"Stacl Trace: {exception?.StackTrace}");
};

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AppMappingProfile)); //регистрация AppMappingProfile

builder.Services.AddDbContext<Knitted_Toys_StoreDBContext>(
    options =>
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
    options.AddPolicy("AlowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; //10MB
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

app.UseHttpsRedirection();
app.UseCors("AlowFrontend");
app.UseAuthorization();
app.UseCartIdentifier();

app.MapControllers();

app.Run();
