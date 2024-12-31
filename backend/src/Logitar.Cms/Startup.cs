using Logitar.Cms.Core;
using Logitar.Cms.Extensions;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.SqlServer;
using Logitar.Cms.Settings;
using Logitar.Cms.Web;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.FeatureManagement;
using Scalar.AspNetCore;

namespace Logitar.Cms;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddLogitarCmsCore();
    services.AddLogitarCmsInfrastructure();
    services.AddLogitarCmsWeb();

    CorsSettings corsSettings = _configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddOpenApi();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.SqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.SqlServer:
        services.AddLogitarCmsWithSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    services.AddFeatureManagement();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      ConfigureAsync(application).Wait();
    }
  }
  public virtual async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();

    if (await featureManager.IsEnabledAsync(FeatureFlags.UseScalarUI))
    {
      application.MapOpenApi();
      application.MapScalarApiReference();
    }

    application.UseHttpsRedirection();
    application.UseCors();
    application.UseStaticFiles();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
