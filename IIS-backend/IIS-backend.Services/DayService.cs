using IIS_backend.DataAccess;
using IIS_backend.Domain.Entities;
using IIS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIS_backend.Services;

public class DayService : IDayService
{
    private readonly Context _context;

    public DayService(Context context)
    {
        _context = context;
    }

    public Task<List<Day>> GetAll()
    {
        return _context.Days.ToListAsync();
    }

    public Task<Day?> GetById(long id)
    {
        return _context.Days.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Day> Create(Day day)
    {
        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        day.CompetitionId = competition.Id;
        _context.Days.Add(day);
        await _context.SaveChangesAsync();
        return day;
    }

    public async Task<Day> Update(Day day)
    {
        var fromDb = await _context.Days.FirstOrDefaultAsync(x => x.Id == day.Id);
        if (fromDb == null) throw new Exception("Day not found.");

        if (fromDb.Date.Date < DateTime.UtcNow.Date)
            throw new Exception("Izmena dana koji je prošao nije moguća.");

        // ako ima ticket items, ne dozvoli update (isto kao primer)
        if (await _context.TicketItems.AnyAsync(x => x.DayId == day.Id && x.Ticket.Status == TicketStatus.Active))
            throw new Exception("Izmena dana koji sadrži aktivne karte nije moguća.");

        fromDb.Date = day.Date;
        fromDb.BasePrice = day.BasePrice;

        await _context.SaveChangesAsync();
        return fromDb;
    }

    public async Task Delete(long id)
    {
        var fromDb = await _context.Days.FirstOrDefaultAsync(x => x.Id == id);
        if (fromDb == null) throw new Exception("Day not found.");

        try
        {
            _context.Days.Remove(fromDb);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception("Brisanje dana koji sadrži karte nije moguće.");
        }
    }
}