using Knitted_Toys_Store.Contracts;
using Knitted_Toys_Store.Domain.Models.Domain;

namespace Knitted_Toys_Store.App.Services
{
    public interface IToyService
    {
        Task<Guid> CreateToyAsync(ToysRequest request);
        Task<Guid> DeleteToyAsync(Guid id);
        Task<IEnumerable<ToysResponse>> GetAllToysAsync();
        Task<ToysResponse?> GetToyByIdAsync(Guid id);
        Task<Guid> UpdateToyAsync(Guid id, ToysRequest request);
        //Task<Guid> UpdateToyAsync(Guid id, string name, string description, string size, decimal price, string imageUrl);
    }
}