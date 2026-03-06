using DayEntity= IIS_backend.Domain.Entities.Day;

namespace IIS_backend.DTOs.Day;

public static class DayDtoExtensions
{
    public static DayEntity ToEntity(this CreateDayDto dto) => new()
    {
        Date = dto.Date,
        BasePrice = dto.BasePrice
    };

    public static DayEntity ToEntity(this UpdateDayDto dto) => new()
    {
        Date = dto.Date,
        BasePrice = dto.BasePrice
    };

    public static GetDayDto ToDto(this DayEntity d) => new()
    {
        Id = d.Id,
        Date = d.Date,
        BasePrice = d.BasePrice
    };
}