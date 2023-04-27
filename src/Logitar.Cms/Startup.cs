using Logitar.Cms.EntityFrameworkCore.PostgreSQL;
using Logitar.Cms.Schema;
using Logitar.Cms.Web;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Middlewares;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

namespace Logitar.Cms;

internal class Startup : StartupBase
{
  private const string DatabaseProviderKey = "DatabaseProvider";

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

    services.AddLogitarCmsSchema(_configuration);
    services.AddLogitarCmsWeb();

    services.AddCors(_configuration);

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>(DatabaseProviderKey)
      ?? throw new InvalidOperationException($"The configuration key '{DatabaseProviderKey}' could not be found.");
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarCmsEntityFrameworkCorePostgreSQLStore(_configuration);
        healthChecks.AddDbContextCheck<CmsContext>();
        healthChecks.AddDbContextCheck<EventContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    if (_configuration.GetValue<bool>("UseGraphQLAltair"))
    {
      builder.UseGraphQLAltair();
    }
    if (_configuration.GetValue<bool>("UseGraphQLGraphiQL"))
    {
      builder.UseGraphQLGraphiQL();
    }
    if (_configuration.GetValue<bool>("UseGraphQLPlayground"))
    {
      builder.UseGraphQLPlayground();
    }
    if (_configuration.GetValue<bool>("UseGraphQLVoyager"))
    {
      builder.UseGraphQLVoyager();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<RefreshSession>();
    builder.UseGraphQL<CmsSchema>();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
