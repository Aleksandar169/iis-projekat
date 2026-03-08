using IIS_backend.Services.Interfaces;

namespace IIS_backend.DTOs.Ticket;

public class CalculateTicketDto
{
    public List<TicketSelectionDto> Selections { get; set; } = new();
    public string? PromoCode { get; set; }
    public long SelectedCurrencyId { get; set; }
}

public class CalculateTicketResponseDto
{
    public List<CalculatedTicketItemDto> Items { get; set; } = new();

    public decimal SubtotalRsd { get; set; }
    public bool DateDiscountApplied { get; set; }
    public decimal DateDiscountAmountRsd { get; set; }

    public bool PromoDiscountApplied { get; set; }
    public decimal PromoDiscountAmountRsd { get; set; }

    public decimal TotalAfterDiscountsRsd { get; set; }

    public long SelectedCurrencyId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public decimal FinalAmount { get; set; }
}

public class CalculatedTicketItemDto
{
    public long DayId { get; set; }
    public string DayDate { get; set; } = string.Empty;
    public decimal BasePriceRsd { get; set; }

    public long ZoneId { get; set; }
    public string ZoneCharacteristics { get; set; } = string.Empty;
    public decimal ZoneAddonRsd { get; set; }

    public decimal ItemTotalRsd { get; set; }
}

public static class TicketCalculationDtoExtensions
{
    public static CalculateTicketResponseDto ToDto(this TicketCalculationResult result)
    {
        return new CalculateTicketResponseDto
        {
            Items = result.Items
                .OrderBy(x => x.DayDate)
                .Select(x => new CalculatedTicketItemDto
                {
                    DayId = x.DayId,
                    DayDate = x.DayDate.ToString("yyyy-MM-dd"),
                    BasePriceRsd = x.BasePriceRsd,
                    ZoneId = x.ZoneId,
                    ZoneCharacteristics = x.ZoneCharacteristics,
                    ZoneAddonRsd = x.ZoneAddonRsd,
                    ItemTotalRsd = x.ItemTotalRsd
                })
                .ToList(),

            SubtotalRsd = result.SubtotalRsd,
            DateDiscountApplied = result.DateDiscountApplied,
            DateDiscountAmountRsd = result.DateDiscountAmountRsd,
            PromoDiscountApplied = result.PromoDiscountApplied,
            PromoDiscountAmountRsd = result.PromoDiscountAmountRsd,
            TotalAfterDiscountsRsd = result.TotalAfterDiscountsRsd,
            SelectedCurrencyId = result.SelectedCurrencyId,
            CurrencyCode = result.CurrencyCode,
            ExchangeRate = result.ExchangeRate,
            FinalAmount = result.FinalAmount
        };
    }
}