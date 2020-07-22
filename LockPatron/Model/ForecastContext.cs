using Microsoft.EntityFrameworkCore;

namespace LockPatron.Model
{
  public class ForecastContext : DbContext
  {
    public ForecastContext(DbContextOptions<ForecastContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> Forecasts { get; set; }
  }
}
