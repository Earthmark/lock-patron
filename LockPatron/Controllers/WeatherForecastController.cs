using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LockPatron.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LockPatron.Controllers
{
  [ApiController]
  [Route("[controller]")]
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

    [HttpGet]
    public IQueryable<WeatherForecast> Get([FromQuery, Range(0, 50)] int count)
    {
      return _ctx.Forecasts.Take(count);
    }

    [HttpPost]
    public async Task<WeatherForecast> Post()
    {
      _logger.LogTrace("Adding new forecast.");
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
