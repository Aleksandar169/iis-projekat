namespace IIS_backend.DTOs.Zone;

public class CreateZoneDto
{
    public int Capacity { get; set; }
    public string Characteristics { get; set; } = string.Empty;
    public decimal PriceAddon { get; set; }
}