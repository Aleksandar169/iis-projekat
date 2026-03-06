using ZoneEntity=IIS_backend.Domain.Entities.Zone;

namespace IIS_backend.DTOs.Zone;

public static class ZoneDtoExtensions
{
    public static ZoneEntity ToEntity(this CreateZoneDto dto) => new()
    {
        Capacity = dto.Capacity,
        Characteristics = dto.Characteristics,
        PriceAddon = dto.PriceAddon
    };

    public static ZoneEntity ToEntity(this UpdateZoneDto dto) => new()
    {
        Capacity = dto.Capacity,
        Characteristics = dto.Characteristics,
        PriceAddon = dto.PriceAddon
    };

    public static GetZoneDto ToDto(this ZoneEntity z) => new()
    {
        Id = z.Id,
        Capacity = z.Capacity,
        Characteristics = z.Characteristics,
        PriceAddon = z.PriceAddon
    };
}