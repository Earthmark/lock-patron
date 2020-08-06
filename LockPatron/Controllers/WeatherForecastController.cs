using System;
using System.Threading.Tasks;
using LockPatron.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LockPatron.Controllers
{
  [ApiController]
  [Route("/")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries =
    {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ForecastContext _ctx;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ForecastContext ctx)
    {
      _logger = logger;
      _ctx = ctx;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeatherForecast>> Get(int id)
    {
      var forecast = await _ctx.Forecasts.SingleOrDefaultAsync(s => s.Key == id);
      if (forecast == null)
      {
        return NotFound(id);
      }

      return Ok(forecast); 
    }

    [HttpPost]
    public async Task<WeatherForecast> Post()
    {
      var rng = new Random();
      var entity = new WeatherForecast
      {
        Date = DateTime.Now.AddDays(rng.Next(0, 30)),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
      };
      _ctx.Forecasts.Add(entity);
      await _ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return entity;
    }
  }
}
