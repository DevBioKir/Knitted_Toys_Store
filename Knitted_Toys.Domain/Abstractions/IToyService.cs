using Knitted_Toys_Store.Domain.Models.Domain;

//namespace Knitted_Toys_Store.Domain.Abstractions
namespace Knitted_Toys_Store.App.Services
{
    public interface IToyService
    {
        Task<Guid> CreateToyAsync(Toy toy);
        Task<Guid> DeleteToysAsync(Guid id);
        Task<List<Toy>> GetAllToysAsync();
        Task<Guid> UpdateToyAsync(Guid id, string name, string description, string size, decimal price, string imageUrl);
    }
}