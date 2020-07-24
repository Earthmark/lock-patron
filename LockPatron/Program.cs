using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LockPatron
{
  /// <summary>
  /// The entry class for the program.
  /// </summary>
  public class Program
  {
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// This creates a host builder, meaning it makes an object that you stick services into,
    /// when your done registering services you say Build and it interconnects them, and then you say
    /// Run for it to find all services implementing <see cref="IHostedService"/>
    /// and calls the <see cref="IHostedService.StartAsync"/> method on them (possibly in a random order).
    /// The Run call blocks until a service inside Host injects and calls <see cref="IHostApplicationLifetime.StopApplication"/>
    /// (A service is by default added to the host that does this if the process is sent a CTL-C,
    /// which is how you normally shutdown the server).
    /// At this point it calls <see cref="IHostedService.StopAsync"/> on every <see cref="IHostedService"/> and returns.
    /// </summary>
    /// <remarks>
    /// This method is normally used for testing, where you remove some services from the injector (such as the real database)
    /// and register fake ones like an in memory database.
    ///
    /// See https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-3.1 for more.
    /// </remarks>
    /// <param name="args">The command line arguments to load into the config system.</param>
    /// <returns>The configured host builder</returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
          webBuilder.UseStartup<Startup>()
        );
  }
}
