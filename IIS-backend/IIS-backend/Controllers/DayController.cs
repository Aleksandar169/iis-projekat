using IIS_backend.Caching;
using IIS_backend.DTOs.Day;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DayController : ControllerBase
{
    private readonly IDayService _dayService;
    private readonly ICacheService _cache;

    public DayController(IDayService dayService, ICacheService cache)
    {
        _dayService = dayService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<List<GetDayDto>> Get()
    {
        var list = await _dayService.GetAll();
        return list.Select(x => x.ToDto()).ToList();
    }

    [HttpGet("{id:long}")]
    public async Task<GetDayDto?> GetById(long id)
    {
        var d = await _dayService.GetById(id);
        return d?.ToDto();
    }

    [HttpPost]
    public async Task<GetDayDto> Post([FromBody] CreateDayDto dto)
    {
        var created = await _dayService.Create(dto.ToEntity());
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
        return created.ToDto();
    }

    [HttpPut("{id:long}")]
    public async Task<GetDayDto> Put(long id, [FromBody] UpdateDayDto dto)
    {
        var entity = dto.ToEntity();
        entity.Id = id;
        var updated = await _dayService.Update(entity);
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
        return updated.ToDto();
    }

    [HttpDelete("{id:long}")]
    public async Task Delete(long id)
    {
        await _dayService.Delete(id);
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
    }
}