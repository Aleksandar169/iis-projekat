using IIS_backend.DataAccess;
using IIS_backend.Domain.Entities;
using IIS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIS_backend.Services;

public class ZoneService : IZoneService
{
    private readonly Context _context;

    public ZoneService(Context context)
    {
        _context = context;
    }

    public Task<List<Zone>> GetAll()
    {
        return _context.Zones.ToListAsync();
    }

    public Task<Zone?> GetById(long id)
    {
        return _context.Zones.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Zone> Create(Zone zone)
    {
        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        zone.CompetitionId = competition.Id;
        _context.Zones.Add(zone);
        await _context.SaveChangesAsync();
        return zone;
    }

    public async Task<Zone> Update(Zone zone)
    {
        var fromDb = await _context.Zones.FirstOrDefaultAsync(x => x.Id == zone.Id);
        if (fromDb == null) throw new Exception("Zone not found.");


        if (await _context.TicketItems.AnyAsync(x => x.ZoneId == zone.Id && x.Ticket.Status == TicketStatus.Active))
            throw new Exception("Izmena zone koja sadrži aktivne karte nije moguća.");

        fromDb.Capacity = zone.Capacity;
        fromDb.Characteristics = zone.Characteristics;
        fromDb.PriceAddon = zone.PriceAddon;

        await _context.SaveChangesAsync();
        return fromDb;
    }

    public async Task Delete(long id)
    {
        var fromDb = await _context.Zones.FirstOrDefaultAsync(x => x.Id == id);
        if (fromDb == null) throw new Exception("Zone not found.");

        try
        {
            _context.Zones.Remove(fromDb);
            await _context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception("Brisanje zone koja sadrži karte nije moguće.");
        }
    }
}