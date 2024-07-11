using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Extensions;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.MongoDB;
using Logitar.Cms.Settings;
using Logitar.Cms.Web;
using Logitar.Cms.Web.Middlewares;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;

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

    services.AddLogitarCmsWeb(_configuration);

    CorsSettings corsSettings = _configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarCmsWithEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
    services.AddLogitarCmsWithMongoDB(_configuration); // NOTE(fpion): needs to be placed after the relational database to override the LogRepository if MongoDB settings are provided.
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<Logging>();
    // TODO(fpion): RenewSession
    builder.UseMiddleware<RedirectNotFound>();
    builder.UseAuthentication();
    builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
