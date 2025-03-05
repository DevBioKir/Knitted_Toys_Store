using Knitted_Toys_Store.Domain.Models.Domain;

//namespace Knitted_Toys_Store.DataAccess.Repositories
namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface IToysRepositories
    {
        Task<Guid> CreateToyAsync(Toy toy);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<Toy>> GetAllToysAsync();
        Task<Guid> UpdateAsync(Guid id, string name, string description, string size, decimal price, string imageUrl);
    }
}