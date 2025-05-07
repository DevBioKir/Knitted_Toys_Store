using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace Knitted_Toys_Store.App.Services
{
    public class ToyService : IToyService
    {
        private readonly IToysRepositories _toyRepository;
        private readonly ILogger<ToyService> _logger;

        public ToyService(IToysRepositories toyRepository,ILogger<ToyService> logger)
        {
            _toyRepository = toyRepository;
            _logger = logger;
        }

        public async Task<List<Toy>> GetAllToysAsync() //получение списка всех игрушек
        {
            return await _toyRepository.GetAllToysAsync();
        }

        public async Task<Guid> CreateToyAsync(Toy toy)
        {
            return await _toyRepository.CreateToyAsync(toy);
        }

        public async Task<Guid> UpdateToyAsync(Guid id, string name, string description, string size, decimal price, string imageUrl)
        {
            return await _toyRepository.UpdateAsync(id, name, description, size, price, imageUrl);
        }

        public async Task<Guid> DeleteToysAsync(Guid id)
        {
            return await _toyRepository.DeleteAsync(id);
        }

        public async Task<Toy?> GetToyByIdAsync(Guid id)
        {
            return await _toyRepository.GetToyByIdAsync(id);
        }
    }
}
