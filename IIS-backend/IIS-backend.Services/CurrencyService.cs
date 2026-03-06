using IIS_backend.DataAccess;
using IIS_backend.Domain.Entities;
using IIS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIS_backend.Services;

public class CurrencyService : ICurrencyService
{
    private readonly Context _context;

    public CurrencyService(Context context)
    {
        _context = context;
    }

    public Task<List<Currency>> GetAll()
    {
        return _context.Currencies.ToListAsync();
    }
}