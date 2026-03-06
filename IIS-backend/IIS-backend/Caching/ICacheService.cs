namespace IIS_backend.Caching;
using System.Threading.Tasks;

public interface ICacheService
{
    Task<T?> GetRecord<T>(string key);
    Task<bool> SetRecord<T>(string key, T data);
    Task<bool> DeleteRecord(string key);
}