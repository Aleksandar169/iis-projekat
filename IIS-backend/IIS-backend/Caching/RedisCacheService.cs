using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace IIS_backend.Caching;

public class RedisCacheService : ICacheService
{

    private readonly StackExchange.Redis.IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public Task<T?> GetRecord<T>(string key)
    {
        var value = _db.StringGet(key);
        if (value.IsNullOrEmpty) return Task.FromResult<T?>(default);

        var obj = JsonSerializer.Deserialize<T>(value!);
        return Task.FromResult(obj);
    }

    public Task<bool> SetRecord<T>(string key, T data)
    {
        var json = JsonSerializer.Serialize(data);
        return Task.FromResult(_db.StringSet(key, json));
    }

    public Task<bool> DeleteRecord(string key)
    {
        return Task.FromResult(_db.KeyDelete(key));
    }
}