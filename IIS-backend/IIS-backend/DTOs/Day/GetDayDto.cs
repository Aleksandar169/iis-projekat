using System;

namespace IIS_backend.DTOs.Day;

public class GetDayDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal BasePrice { get; set; }
}