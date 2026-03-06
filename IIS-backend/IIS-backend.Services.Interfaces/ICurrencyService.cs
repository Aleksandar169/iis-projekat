using IIS_backend.Domain.Entities;

namespace IIS_backend.Services.Interfaces;

public interface ICurrencyService
{
    Task<List<Currency>> GetAll();
}