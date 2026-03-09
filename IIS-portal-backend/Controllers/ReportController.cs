
using IIS_portal_backend.DataAccess.IIS_portal_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IIS_portal_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly PortalDbContext _context;

        public ReportController(PortalDbContext context)
        {
            _context = context;
        }

      
        [HttpGet("tickets-by-race-day")]
     
        public async Task<IActionResult> GetTicketsByRaceDay()
        {
    
            var rawData = await _context.TicketSales
                .GroupBy(t => t.CompetitionDay)
                .Select(g => new
                {
                    Day = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var formattedData = rawData
                .Select(x => new
                {
                    Day = x.Day.ToString("yyyy-MM-dd"),
                    Count = x.Count
                })
                .OrderBy(x => x.Day)
                .ToList();

            return Ok(formattedData);
        }

        [HttpGet("purchases-by-date")]
    
        public async Task<IActionResult> GetPurchasesByDate()
        {

            var rawData = await _context.TicketSales
                .GroupBy(t => t.PurchaseDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Select(x => x.TicketId).Distinct().Count()
                })
                .ToListAsync();

           
            var formattedData = rawData
                .Select(x => new
                {
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    Count = x.Count
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(formattedData);
        }
    }
}