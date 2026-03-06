using IIS_backend.Caching;
using IIS_backend.DTOs.Zone;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZoneController : ControllerBase
{
    private readonly IZoneService _zoneService;
    private readonly ICacheService _cache;

    public ZoneController(IZoneService zoneService, ICacheService cache)
    {
        _zoneService = zoneService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<List<GetZoneDto>> Get()
    {
        var list = await _zoneService.GetAll();
        return list.Select(x => x.ToDto()).ToList();
    }

    [HttpGet("{id:long}")]
    public async Task<GetZoneDto?> GetById(long id)
    {
        var z = await _zoneService.GetById(id);
        return z?.ToDto();
    }

    [HttpPost]
    public async Task<GetZoneDto> Post([FromBody] CreateZoneDto dto)
    {
        var created = await _zoneService.Create(dto.ToEntity());
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
        return created.ToDto();
    }

    [HttpPut("{id:long}")]
    public async Task<GetZoneDto> Put(long id, [FromBody] UpdateZoneDto dto)
    {
        var entity = dto.ToEntity();
        entity.Id = id;
        var updated = await _zoneService.Update(entity);
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
        return updated.ToDto();
    }

    [HttpDelete("{id:long}")]
    public async Task Delete(long id)
    {
        await _zoneService.Delete(id);
        await _cache.DeleteRecord(CompetitionController.CompetitionBasicInfoCacheKey);
    }
}