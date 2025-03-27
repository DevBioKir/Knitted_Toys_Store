using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.DataAccess;
using Knitted_Toys_Store.App.Mapping;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //app.MapOpenApi();
}

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthorization();
//app.Swagger();

app.MapControllers();

app.Run();
