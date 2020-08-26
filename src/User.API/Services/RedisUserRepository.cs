using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Models;
using IDatabase = StackExchange.Redis.IDatabase;

namespace User.API.Services
{
    public class RedisUserRepository : IUserRepository
    {
        private readonly ILogger<RedisUserRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisUserRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisUserRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(k => k.ToString());
        }

        public async Task<UserTage> GetBasketAsync(int userId)
        {
            var data = await _database.StringGetAsync(userId.ToString());

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UserTage>(data);
        }

        public async Task<UserTage> UpdateBasketAsync(UserTage basket)
        {
            var created = await _database.StringSetAsync(basket.AppUserId.ToString(), JsonConvert.SerializeObject(basket));

            if (!created)
            {
                _logger.LogInformation("Problem occur persisting the item.");
                return null;
            }

            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetBasketAsync(basket.AppUserId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}
