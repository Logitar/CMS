using Logitar.Cms.Core;
using Logitar.Cms.Extensions;
using Logitar.Cms.Middlewares;
using Logitar.Cms.Settings;
using Logitar.Cms.Web;

namespace Logitar.Cms;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    // TODO(fpion): Authentication

    // TODO(fpion): Authorization

    //services.AddDistributedMemoryCache(); // TODO(fpion): Session

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddTransient<IRequestPipeline, HttpRequestPipeline>();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    // TODO(fpion): Persistence

    // TODO(fpion): GraphQL
    services.AddLogitarCmsWeb();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    // TODO(fpion): GraphQL UIs

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    //builder.UseSession(); builder.UseMiddleware<RenewSession>(); // TODO(fpion): Session
    builder.UseMiddleware<RedirectNotFound>();
    // TODO(fpion): Authentication
    // TODO(fpion): Authorization

    // TODO(fpion): GraphQL

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
