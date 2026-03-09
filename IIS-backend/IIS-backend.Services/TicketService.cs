using IIS_backend.DataAccess;
using IIS_backend.Domain.Entities;
using IIS_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIS_backend.Services;

public class TicketService : ITicketService
{
    private readonly Context _context;
    private readonly IExchangeRateClient _exchangeRateClient;

    public TicketService(Context context, IExchangeRateClient exchangeRateClient)
    {
        _context = context;
        _exchangeRateClient = exchangeRateClient;
    }

    public Task<Ticket?> GetByEmailAndCode(string email, string ticketCode)
    {
        return _context.Tickets
            .Include(x => x.TicketItems)
                .ThenInclude(ti => ti.Day)
            .Include(x => x.TicketItems)
                .ThenInclude(ti => ti.Zone)
            .Include(x => x.PaymentSnapshot)
                .ThenInclude(ps => ps.SelectedCurrency)
            .Include(x => x.IssuedPromoCode)
            .Include(x => x.UsedPromoCode)
            .FirstOrDefaultAsync(x => x.Email == email && x.TicketCode == ticketCode);
    }

    public Task<TicketCreationRequestError?> GetError(string email, string ticketCode)
    {
        return _context.TicketCreationRequestErrors
            .FirstOrDefaultAsync(x => x.Email == email && x.TicketCode == ticketCode);
    }

    public async Task<TicketCalculationResult> CalculatePreview(List<TicketSelection> selections, string? promoCode, long selectedCurrencyId){
        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        var allowedCurrencyIds = await _context.CompetitionCurrencies
            .Where(x => x.CompetitionId == competition.Id)
            .Select(x => x.CurrencyId)
            .ToListAsync();

        if (!allowedCurrencyIds.Contains(selectedCurrencyId))
            throw new Exception("Odabrana valuta nije dozvoljena za ovo takmičenje.");

        var currency = await _context.Currencies.FirstOrDefaultAsync(x => x.Id == selectedCurrencyId);
        if (currency == null) throw new Exception("Valuta ne postoji.");

        if (selections == null || selections.Count == 0)
            throw new Exception("Mora se izabrati bar jedan dan i zona.");

        if (selections.Select(s => s.DayId).Distinct().Count() != selections.Count)
            throw new Exception("Za isti dan ne možeš izabrati više zona u jednoj karti.");

        var items = new List<TicketCalculationItem>();
        decimal subtotalRsd = 0m;

        foreach (var sel in selections)
        {
            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == sel.DayId);
            if (day == null) throw new Exception($"Dan {sel.DayId} nije pronađen.");

            var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == sel.ZoneId);
            if (zone == null) throw new Exception($"Zona {sel.ZoneId} nije pronađena.");

            if (day.CompetitionId != competition.Id || zone.CompetitionId != competition.Id)
                throw new Exception("Dan i/ili zona ne pripadaju ovom takmičenju.");

            var activeCount = await _context.TicketItems
                .Include(ti => ti.Ticket)
                .CountAsync(ti => ti.DayId == sel.DayId && ti.ZoneId == sel.ZoneId && ti.Ticket.Status == TicketStatus.Active);

            if (activeCount >= zone.Capacity)
                throw new Exception("Nema slobodnih mesta u izabranoj zoni za taj dan.");

            var itemTotal = day.BasePrice + zone.PriceAddon;
            subtotalRsd += itemTotal;

            items.Add(new TicketCalculationItem
            {
                DayId = day.Id,
                DayDate = day.Date,
                BasePriceRsd = day.BasePrice,
                ZoneId = zone.Id,
                ZoneCharacteristics = zone.Characteristics,
                ZoneAddonRsd = zone.PriceAddon,
                ItemTotalRsd = itemTotal
            });
        }

        bool dateDiscountApplied = DateTime.UtcNow.Date <= competition.DiscountValidUntil.Date;
        decimal dateDiscountAmountRsd = dateDiscountApplied ? subtotalRsd * 0.1m : 0m;

        decimal afterDateDiscountRsd = subtotalRsd - dateDiscountAmountRsd;

        bool promoDiscountApplied = false;
        decimal promoDiscountAmountRsd = 0m;

        if (!string.IsNullOrWhiteSpace(promoCode))
        {
            var usedPromo = await _context.PromoCodes.FirstOrDefaultAsync(x =>
                x.Code == promoCode && x.Status == PromoCodeStatus.Active);

            if (usedPromo == null)
                throw new Exception("Promo kod nije validan.");

            promoDiscountApplied = true;
            promoDiscountAmountRsd = afterDateDiscountRsd * 0.05m;
        }

        decimal totalAfterDiscountsRsd = afterDateDiscountRsd - promoDiscountAmountRsd;

        decimal exchangeRate;
        decimal finalAmount;

        if (currency.Code.Equals("RSD", StringComparison.OrdinalIgnoreCase))
        {
            exchangeRate = 1m;
            finalAmount = totalAfterDiscountsRsd;
        }
        else
        {
            var rate = await _exchangeRateClient.GetRateAsync(currency.Code);

            exchangeRate = rate;
            finalAmount = totalAfterDiscountsRsd / rate;
        }

        return new TicketCalculationResult
        {
            Items = items.OrderBy(x => x.DayDate).ToList(),
            SubtotalRsd = decimal.Round(subtotalRsd, 2),
            DateDiscountApplied = dateDiscountApplied,
            DateDiscountAmountRsd = decimal.Round(dateDiscountAmountRsd, 2),
            PromoDiscountApplied = promoDiscountApplied,
            PromoDiscountAmountRsd = decimal.Round(promoDiscountAmountRsd, 2),
            TotalAfterDiscountsRsd = decimal.Round(totalAfterDiscountsRsd, 2),
            SelectedCurrencyId = currency.Id,
            CurrencyCode = currency.Code,
            ExchangeRate = exchangeRate,
            FinalAmount = decimal.Round(finalAmount, 2)
        };
    }

    public async Task<Ticket> CreateTicket(Ticket ticket, List<TicketSelection> selections, string? promoCode, long selectedCurrencyId)
    {
        try
        {
            if (ticket.Id != 0) throw new Exception("Nova karta ne sme imati postojeći ID.");
            if (ticket.TicketItems.Count > 0) throw new Exception("Nova karta ne sme imati unapred vezane stavke.");

            var preview = await CalculatePreview(selections, promoCode, selectedCurrencyId);

            var currency = await _context.Currencies.FirstOrDefaultAsync(x => x.Id == selectedCurrencyId);
            if (currency == null) throw new Exception("Valuta ne postoji.");

            foreach (var sel in selections)
            {
                ticket.TicketItems.Add(new TicketItem
                {
                    DayId = sel.DayId,
                    ZoneId = sel.ZoneId
                });
            }

            PromoCode? usedPromo = null;
            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                usedPromo = await _context.PromoCodes.FirstOrDefaultAsync(x =>
                    x.Code == promoCode && x.Status == PromoCodeStatus.Active);

                if (usedPromo == null)
                    throw new Exception("Promo kod nije validan.");

                usedPromo.Status = PromoCodeStatus.Used;
                usedPromo.UsageDate = DateTime.UtcNow;

                ticket.UsedPromoCodeId = usedPromo.Id;
            }

            var paymentSnapshot = new PaymentSnapshot
            {
                SelectedCurrencyId = currency.Id,
                ExchangeRate = preview.ExchangeRate,
                FinalAmount = preview.FinalAmount
            };

            _context.PaymentSnapshots.Add(paymentSnapshot);
            await _context.SaveChangesAsync();

            ticket.PaymentSnapshotId = paymentSnapshot.Id;
            ticket.PurchaseDate = DateTime.UtcNow;
            ticket.Status = TicketStatus.Active;

            var issued = new PromoCode
            {
                Code = Guid.NewGuid().ToString("N")[..8],
                Status = PromoCodeStatus.Active
            };

            _context.PromoCodes.Add(issued);
            await _context.SaveChangesAsync();

            ticket.IssuedPromoCodeId = issued.Id;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }
        catch (Exception ex)
        {
            _context.ChangeTracker.Clear();
            _context.TicketCreationRequestErrors.Add(new TicketCreationRequestError
            {
                Email = ticket.Email,
                TicketCode = ticket.TicketCode,
                ErrorMessage = ex.Message
            });
            await _context.SaveChangesAsync();
            throw;
        }
    }

    public async Task<Ticket> AddSelections(string email, string ticketCode, List<TicketSelection> add)
    {
        var ticket = await _context.Tickets
            .Include(x => x.TicketItems)
            .Include(x => x.PaymentSnapshot)
            .Include(x => x.IssuedPromoCode)
            .Include(x => x.UsedPromoCode)
            .FirstOrDefaultAsync(x => x.Email == email && x.TicketCode == ticketCode);

        if (ticket == null) throw new Exception("Karta nije pronađena.");
        if (ticket.Status != TicketStatus.Active) throw new Exception("Karta je otkazana.");

        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        var existingDayIds = ticket.TicketItems.Select(ti => ti.DayId).ToHashSet();
        if (add.Any(a => existingDayIds.Contains(a.DayId)))
            throw new Exception("Ne možeš dodati dan koji već postoji na karti.");

        foreach (var sel in add)
        {
            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == sel.DayId);
            if (day == null) throw new Exception("Dan nije pronađen.");

            var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == sel.ZoneId);
            if (zone == null) throw new Exception("Zona nije pronađena.");

            if (day.CompetitionId != competition.Id || zone.CompetitionId != competition.Id)
                throw new Exception("Dan i/ili zona ne pripadaju ovom takmičenju.");

            var activeCount = await _context.TicketItems
                .Include(ti => ti.Ticket)
                .CountAsync(ti => ti.DayId == sel.DayId && ti.ZoneId == sel.ZoneId && ti.Ticket.Status == TicketStatus.Active);

            if (activeCount >= zone.Capacity)
                throw new Exception("Nema slobodnih mesta u izabranoj zoni za taj dan.");

            ticket.TicketItems.Add(new TicketItem { DayId = sel.DayId, ZoneId = sel.ZoneId });
        }

        await RecalculatePayment(ticket, competition);
        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task<Ticket> RemoveSelections(string email, string ticketCode, List<long> dayIdsToRemove)
    {
        var ticket = await _context.Tickets
            .Include(x => x.TicketItems)
            .Include(x => x.PaymentSnapshot)
            .Include(x => x.IssuedPromoCode)
            .Include(x => x.UsedPromoCode)
            .FirstOrDefaultAsync(x => x.Email == email && x.TicketCode == ticketCode);

        if (ticket == null) throw new Exception("Karta nije pronađena.");
        if (ticket.Status != TicketStatus.Active) throw new Exception("Karta je otkazana.");

        if (dayIdsToRemove == null || dayIdsToRemove.Count == 0)
            throw new Exception("Moraš navesti bar jedan dan za uklanjanje.");

        var toRemove = ticket.TicketItems.Where(ti => dayIdsToRemove.Contains(ti.DayId)).ToList();
        if (toRemove.Count == 0) throw new Exception("Nijedan od navedenih dana nije na karti.");

        if (ticket.TicketItems.Count - toRemove.Count <= 0)
            throw new Exception("Karta mora sadržati bar jedan dan.");

        _context.TicketItems.RemoveRange(toRemove);

        var competition = await _context.Competitions.FirstOrDefaultAsync();
        if (competition == null) throw new Exception("Competition not found.");

        await RecalculatePayment(ticket, competition);
        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task<Ticket> Cancel(string email, string ticketCode)
    {
        var ticket = await _context.Tickets
            .Include(x => x.IssuedPromoCode)
            .Include(x => x.TicketItems)
                .ThenInclude(x => x.Day)
            .FirstOrDefaultAsync(x => x.Email == email && x.TicketCode == ticketCode);

        if (ticket == null) throw new Exception("Karta nije pronađena.");
        if (ticket.Status == TicketStatus.Cancelled) throw new Exception("Karta je već otkazana.");

        ticket.Status = TicketStatus.Cancelled;
        ticket.IssuedPromoCode.Status = PromoCodeStatus.Inactive;

        await _context.SaveChangesAsync();
        return ticket;
    }

    private async Task RecalculatePayment(Ticket ticket, Competition competition)
    {
        decimal totalRsd = 0m;

        foreach (var item in ticket.TicketItems)
        {
            var day = await _context.Days.FirstOrDefaultAsync(d => d.Id == item.DayId);
            var zone = await _context.Zones.FirstOrDefaultAsync(z => z.Id == item.ZoneId);
            if (day == null || zone == null)
                throw new Exception("Podaci o danu/zoni nisu validni.");

            totalRsd += day.BasePrice + zone.PriceAddon;
        }

        if (DateTime.UtcNow.Date <= competition.DiscountValidUntil.Date)
        {
            totalRsd *= 0.9m;
        }

        if (ticket.UsedPromoCodeId != null)
        {
            totalRsd *= 0.95m;
        }

        var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == ticket.PaymentSnapshot.SelectedCurrencyId);
        if (currency == null) throw new Exception("Valuta nije pronađena.");

        if (currency.Code.Equals("RSD", StringComparison.OrdinalIgnoreCase))
        {
            ticket.PaymentSnapshot.ExchangeRate = 1m;
            ticket.PaymentSnapshot.FinalAmount = decimal.Round(totalRsd, 2);
        }
        else
        {
            var rateRsd = await _exchangeRateClient.GetRateAsync("RSD");
            var totalEur = totalRsd / rateRsd;

            var rateTarget = await _exchangeRateClient.GetRateAsync(currency.Code);
            var finalAmount = totalEur * rateTarget;

            ticket.PaymentSnapshot.ExchangeRate = rateTarget;
            ticket.PaymentSnapshot.FinalAmount = decimal.Round(finalAmount, 2);
        }
    }
}