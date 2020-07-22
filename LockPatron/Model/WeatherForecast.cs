using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockPatron.Model
{
  public class WeatherForecast
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
    public int Key { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }
  }
}
