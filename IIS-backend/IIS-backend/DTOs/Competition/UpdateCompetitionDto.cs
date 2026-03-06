using System;

namespace IIS_backend.DTOs.Competition;

public class UpdateCompetitionDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime DiscountValidUntil { get; set; }
    public string? AdditionalInfo { get; set; }
}