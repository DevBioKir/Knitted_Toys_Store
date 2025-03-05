using Knitted_Toys_Store.Domain.Models.Domain;

//namespace Knitted_Toys_Store.DataAccess.Repositories
namespace Knitted_Toys_Store.Domain.Abstractions
{
    public interface IToysRepositories
    {
        Task<Guid> CreateToy(Toy toy);
        Task<Guid> Delete(Guid id);
        Task<List<Toy>> GetAllToys();
        Task<Guid> Update(Guid id, string name, string description, string size, decimal price, string imageUrl);
    }
}