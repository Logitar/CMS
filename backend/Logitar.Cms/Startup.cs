using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Infrastructure;

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

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(_configuration);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    builder.UseHttpsRedirection();
  }
}
