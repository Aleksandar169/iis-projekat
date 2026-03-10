using IIS_backend.DataAccess;
using IIS_backend.Domain.Entities;
using IIS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIS_backend.Services;

public class CompetitionService : ICompetitionService
{
    private readonly Context _context;

    public CompetitionService(Context context)
    {
        _context = context;
    }

    public Task<Competition?> GetDetails()
    {
        return _context.Competitions
            .Include(x => x.Days)
            .Include(x => x.Zones)
            .Include(x => x.CompetitionCurrencies)
                .ThenInclude(cc => cc.Currency)
            .FirstOrDefaultAsync();
    }

    public async Task<Competition> Upsert(Competition competition)
    {
        var existing = await _context.Competitions.FirstOrDefaultAsync();
        if (existing == null)
        {
            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();
            return competition;
        }

        existing.Name = competition.Name;
        existing.Location = competition.Location;
        existing.StartDate = competition.StartDate;
        existing.EndDate = competition.EndDate;
        existing.DiscountValidUntil = competition.DiscountValidUntil;
        existing.AdditionalInfo = competition.AdditionalInfo;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task SetAllowedCurrencies(List<long> currencyIds)
    {
        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");


        var existing = await _context.CompetitionCurrencies
            .Where(x => x.CompetitionId == competition.Id)
            .ToListAsync();

        _context.CompetitionCurrencies.RemoveRange(existing);


        var currencies = await _context.Currencies.Where(c => currencyIds.Contains(c.Id)).ToListAsync();
        if (currencies.Count != currencyIds.Distinct().Count())
            throw new Exception("Jedna ili više valuta ne postoji.");

        foreach (var cId in currencyIds.Distinct())
        {
            _context.CompetitionCurrencies.Add(new CompetitionCurrency
            {
                CompetitionId = competition.Id,
                CurrencyId = cId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Currency>> GetAllowedCurrencies()
    {
        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        return await _context.CompetitionCurrencies
            .Where(x => x.CompetitionId == competition.Id)
            .Select(x => x.Currency)
            .ToListAsync();
    }
}