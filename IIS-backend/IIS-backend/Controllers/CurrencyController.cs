using IIS_backend.DTOs.Currency;
using IIS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet]
    public async Task<List<CurrencyDto>> Get()
    {
        var list = await _currencyService.GetAll();
        return list.Select(x => x.ToDto()).ToList();
    }
}