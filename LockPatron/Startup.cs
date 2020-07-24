using LockPatron.Formatters;
using LockPatron.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LockPatron
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    /// <summary>
    /// This is the object that contains values pulled from config sources.
    /// Different config sources can override each other, see <see cref="Program.CreateHostBuilder"/> for links to what is loaded in what order.
    /// </summary>
    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // This is the db context for weather forecasts, this will probably get deleted.
      services.AddDbContextPool<ForecastContext>(o => o.UseInMemoryDatabase("Tacos"));

      // This registers services that scan the assembly for classes that define HTTP routes (there's a few flags it looks for).
      services.AddControllers(o =>
      {
        // Inserting at 0 means this applies before all other formatters.
        // Really there are 4 other formatters already in this list that deal with json and such.
        o.OutputFormatters.Insert(0, new UrlEncodedOutputFormatter());
      });

      // Swagger (also called OpenAPI) is a way to expose a REST api to customers,
      // you provide them with the swagger document and they can often generate clients with tools to use your api.
      // It's similar to how GRPC works with protoc, but far more sketchy, but also far more common.
      // This registers services that depend on services added in AddControllers that say what services were registered.
      // From there it formats that data into a json file to be served by a middleware added in the Configure method.
      services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "Lock-Patron",
        Version = "v1"
      }));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // This method defines what happens with a request, in order.
      // This style of structure is often called a middleware stack.
      // Each Use* adds a middleware to the stack,
      // each HTTP request starts at the top of this stack and
      // moves down until the step it is inside says "Yeah, we're done here".
      // From there the request moves back up to the top, where the request is then completed.
      // If a request makes it to the bottom of the method without a middleware saying it was handled then 404 is returned.

      // These middleware load services from the dependency injector.

      if (env.IsDevelopment())
      {
        // This middleware does nothing on the way down,
        // but when it calls the next middleware in the stack
        // it wraps the call in a catch and shows a fancy web page if an exception is thrown.
        // Call stacks are considered bad to leak publicly, hence this being enabled in dev mode only.
        // There is a handler above all middleware defined here that also catches exceptions,
        // but it returns a 500 and gives very little info.
        app.UseDeveloperExceptionPage();
      }

      // This completes any request that comes in with http protocol as a redirect
      // to https protocol via a 3xx status code and a location response header.
      app.UseHttpsRedirection();

      // This is kind of dumb but means that routes are a thing the server can handle.
      app.UseRouting();

      // This is middleware that may validate that a user has valid auth tokens.
      // There are multiple ways to define auth tokens, but in general if authentication fails a 401 is returned.
      // The logic to tell if an auth check needs to happen gets complicated,
      // but is often done via controllers defining the Authorize attribute on the class or on a method.
      app.UseAuthorization();

      // This accepts routes that match any defined swagger apis (in our case /swagger/v1/swagger.json).
      // This data is provided by the services added to the dependency injector in AddSwaggerGen.
      app.UseSwagger();
      // This serves some static web pages at /swagger that consume the swagger json document and make it user friendly.
      // This serves multiple routes, as there's multiple static files being served.
      app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lock Patron V1")
      );

      // This figures out all the routes in the assembly and maps them out.
      app.UseEndpoints(endpoints =>
      {
        // This uses the routing info that was already gathered by services added in AddControllers,
        // from there if it finds a match to the request, it asks the dependency injector for an instance of the controller
        // that contains the http method, and then calls the http method on the controller.
        // The middleware converts from the binary data streams provided by the request to the types requested by the controller,
        // and it converts from the types provided by the controller back to binary data streams.
        endpoints.MapControllers();
      });
    }
  }
}
