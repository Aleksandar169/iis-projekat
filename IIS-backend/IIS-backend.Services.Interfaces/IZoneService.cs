using IIS_backend.Domain.Entities;

namespace IIS_backend.Services.Interfaces;

public interface IZoneService
{
    Task<List<Zone>> GetAll();
    Task<Zone?> GetById(long id);
    Task<Zone> Create(Zone zone);
    Task<Zone> Update(Zone zone);
    Task Delete(long id);
}