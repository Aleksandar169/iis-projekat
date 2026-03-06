using IIS_backend.Domain.Entities;

namespace IIS_backend.Services.Interfaces;

public interface IDayService
{
    Task<List<Day>> GetAll();
    Task<Day?> GetById(long id);
    Task<Day> Create(Day day);
    Task<Day> Update(Day day);
    Task Delete(long id);
}