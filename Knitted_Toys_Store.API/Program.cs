using Knitted_Toys_Store.DataAccess;
using Knitted_Toys_Store.DataAccess.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AppMappingProfile)); //регистрация AppMappingProfile
builder.Services.AddDbContext<Knitted_Toys_StoreDBContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(Knitted_Toys_StoreDBContext)));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
