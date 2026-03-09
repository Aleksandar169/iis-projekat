using IIS_backend.Caching;
using IIS_backend.DTOs.Competition;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetitionController : ControllerBase
{
    public const string CompetitionBasicInfoCacheKey = "competition_basic_info";

    private readonly ICacheService _cache;
    private readonly ICompetitionService _competitionService;

    public CompetitionController(ICacheService cache, ICompetitionService competitionService)
    {
        _cache = cache;
        _competitionService = competitionService;
    }

    [HttpGet]
    public async Task<CompetitionBasicInfoDto> Get()
    {
        var cached = await _cache.GetRecord<CompetitionBasicInfoDto>(CompetitionBasicInfoCacheKey);
        if (cached != null) return cached;

        var competition = await _competitionService.GetDetails();
        if (competition == null) throw new Exception("Competition not found.");

        var allowed = await _competitionService.GetAllowedCurrencies();
        var dto = competition.ToBasicInfoDto(allowed);

        await _cache.SetRecord(CompetitionBasicInfoCacheKey, dto);
        return dto;
    }

    [HttpPut]
    public async Task<CompetitionBasicInfoDto> Put([FromBody] UpdateCompetitionDto dto)
    {
        var updated = await _competitionService.Upsert(dto.ToEntity());
        await _cache.DeleteRecord(CompetitionBasicInfoCacheKey);

        var allowed = await _competitionService.GetAllowedCurrencies();
        return updated.ToBasicInfoDto(allowed);
    }

    [HttpPut("allowed-currencies")]
    public async Task<List<long>> SetAllowed([FromBody] SetAllowedCurrenciesDto dto)
    {
        await _competitionService.SetAllowedCurrencies(dto.CurrencyIds);
        await _cache.DeleteRecord(CompetitionBasicInfoCacheKey);
        return dto.CurrencyIds;
    }

    [HttpGet("allowed-currencies")]
    public async Task<object> GetAllowed()
    {
        var allowed = await _competitionService.GetAllowedCurrencies();
        return allowed.Select(a => new { a.Id, a.CurrencyName, a.Code }).ToList();
    }
}