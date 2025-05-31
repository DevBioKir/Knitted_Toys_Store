using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace Knitted_Toys_Store.Infrastructure.Cash
{
    public abstract class CachedServiceBase
    {
        protected readonly IConnectionMultiplexer _redis;
        protected readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        // Настройки для сериализации доменных моделей
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        protected CachedServiceBase(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected async Task<T?> GetFromCacheAsync<T>(string key) where T : class
        {
            try
            {
                var db = _redis.GetDatabase();
                var cached = await db.StringGetAsync(key);

                if (cached.IsNullOrEmpty)
                {
                    Console.WriteLine($"Кэш пуст для ключа: {key}");
                    return null;
                }

                Console.WriteLine($"JSON из кэша: {cached}"); // Для отладки

                var result = JsonSerializer.Deserialize<T>(cached, _jsonOptions);
                Console.WriteLine($"Десериализация успешна для ключа: {key}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка десериализации кэша для ключа '{key}': {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await InvalidateCacheAsync(key);
                return null;
            }
        }

        protected async Task SetCacheAsync<T>(string key, T data)
        {
            try
            {
                var db = _redis.GetDatabase();
                var serialized = JsonSerializer.Serialize(data, _jsonOptions);

                Console.WriteLine($"Сериализуем в кэш: {serialized}"); // Для отладки

                await db.StringSetAsync(key, serialized, _cacheExpiration);
                Console.WriteLine($"Данные сохранены в кэш с ключом: {key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка сериализации в кэш для ключа '{key}': {ex.Message}");
            }
        }

        protected async Task InvalidateCacheAsync(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(key);
                Console.WriteLine($"Кэш удален для ключа: {key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления кэша для ключа '{key}': {ex.Message}");
            }
        }

        protected async Task InvalidateMultipleCacheAsync(params string[] keys)
        {
            try
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
                Console.WriteLine($"Множественный кэш удален для ключей: {string.Join(", ", keys)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления множественного кэша: {ex.Message}");
            }
        }

        protected async Task<bool> CacheExistsAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }

        protected async Task<TimeSpan?> GetCacheTtlAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyTimeToLiveAsync(key);
        }
    }
}
