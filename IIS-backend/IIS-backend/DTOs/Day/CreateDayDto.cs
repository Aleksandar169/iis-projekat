using System;

namespace IIS_backend.DTOs.Day;

public class CreateDayDto
{
    public DateTime Date { get; set; }
    public decimal BasePrice { get; set; }
}