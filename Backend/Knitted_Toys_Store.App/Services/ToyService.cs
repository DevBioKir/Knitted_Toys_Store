using Knitted_Toys_Store.Domain.Models.Domain;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Knitted_Toys_Store.Infrastructure.Cash;
using StackExchange.Redis;
using Knitted_Toys_Store.Contracts;
using Mapster;
using MapsterMapper;

namespace Knitted_Toys_Store.App.Services
{
    public class ToyService : CachedServiceBase, IToyService
    {
        private readonly IToysRepositories _toyRepository;
        private readonly ILogger<ToyService> _logger;
        private const string TOYS_CACHE_KEY = "toys:all";
        private readonly IMapper _mapper;

        public ToyService(
            IToysRepositories toyRepository,
            ILogger<ToyService> logger,
            IMapper mapper,
            IConnectionMultiplexer redis) : base(redis)
        {
            _toyRepository = toyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ToysResponse>> GetAllToysAsync() //получение списка всех игрушек
        {
            _logger.LogInformation("Попытка получить игрушки из кэша с ключом: {CacheKey}", TOYS_CACHE_KEY);

            // ВРЕМЕННО: очистим кэш для отладки
            await InvalidateCacheAsync(TOYS_CACHE_KEY);
            _logger.LogInformation("Кэш очищен для отладки");

            // Пытаемся получить из кэша
            var cachedToys = await GetFromCacheAsync<List<ToysResponse>>(TOYS_CACHE_KEY);
            if (cachedToys != null)
            {
                _logger.LogInformation("Игрушки найдены в кэше. Количество: {Count}", cachedToys.Count());
                return cachedToys;
            }

            _logger.LogInformation("Игрушки не найдены в кэше. Загружаем из базы данных...");

            // Если в кэше нет, получаем из базы данных
            var domainToys = await _toyRepository.GetAllToysAsync();

            if (domainToys?.Any() == true)
            {
                // Конвертируем доменные модели в DTO через Mapster
                //var toyResponses = domainToys.Adapt<List<ToysResponse>>();
                var toyResponses = _mapper.Map<List<ToysResponse>>(domainToys);

                _logger.LogInformation("Сохраняем {Count} игрушек в кэш на {Minutes} минут", toyResponses.Count, 5);

                // Кэшируем DTO (они легко сериализуются)
                await SetCacheAsync(TOYS_CACHE_KEY, toyResponses);

                return toyResponses;
            }

            _logger.LogInformation("Игрушки не найдены");
            return Enumerable.Empty<ToysResponse>();
        }

        public async Task<Guid> CreateToyAsync(ToysRequest request)
        {
            _logger.LogInformation("Добавляем новую игрушку: {ToyName}", request.Name);

            // Конвертируем DTO в доменную модель через Mapster
            var domainToy = request.Adapt<Toy>();

            var toyId = await _toyRepository.CreateToyAsync(domainToy);
            _logger.LogInformation("Игрушка создана в БД с ID: {ToyId}", toyId);

            _logger.LogInformation("Удаляем устаревший кэш...");
            await InvalidateCacheAsync(TOYS_CACHE_KEY);
            _logger.LogInformation("Кэш очищен!");

            return toyId;
        }

        public async Task<Guid> UpdateToyAsync(Guid id, ToysRequest request)
        {
            _logger.LogInformation("Обновляем игрушку с ID: {ToyId}", id);

            var updatedToyId = await _toyRepository.UpdateAsync(
                id,
                request.Name,
                request.Description,
                request.Size,
                request.Price,
                request.ImageUrl);

            _logger.LogInformation("Игрушка обновлена в БД с ID: {ToyId}", updatedToyId);

            _logger.LogInformation("Удаляем устаревший кэш...");
            await InvalidateMultipleCacheAsync(
                TOYS_CACHE_KEY,
                $"toy:{id}"
            );
            _logger.LogInformation("Кэш очищен!");

            return updatedToyId;
        }

        public async Task<Guid> DeleteToyAsync(Guid id)
        {
            _logger.LogInformation("Удаляем игрушку с ID: {ToyId}", id);

            var deletedToyId = await _toyRepository.DeleteAsync(id);
            _logger.LogInformation("Игрушка удалена из БД с ID: {ToyId}", deletedToyId);

            _logger.LogInformation("Удаляем устаревший кэш...");
            await InvalidateMultipleCacheAsync(
                TOYS_CACHE_KEY,
                $"toy:{id}"
            );
            _logger.LogInformation("Кэш очищен!");

            return deletedToyId;
        }

        public async Task<ToysResponse?> GetToyByIdAsync(Guid id)
        {
            _logger.LogInformation("Ищем игрушку с ID: {ToyId}", id);

            var domainToy = await _toyRepository.GetToyByIdAsync(id);

            if (domainToy != null)
            {
                var toyResponse = domainToy.Adapt<ToysResponse>();

                _logger.LogInformation("Игрушка найдена в БД: {ToyName}", toyResponse.Name);
                return toyResponse;
            }

            _logger.LogWarning("Игрушка с ID {ToyId} не найдена", id);
            return null;
        }
    }
}
