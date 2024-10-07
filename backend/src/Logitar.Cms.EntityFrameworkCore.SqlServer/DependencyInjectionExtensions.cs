using Logitar.Identity.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "SQLCONNSTR_Cms";

  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable(ConfigurationKey);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetValue<string>(ConfigurationKey);
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The configuration '{ConfigurationKey}' could not be found.", nameof(configuration));
    }
    return services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarCmsWithEntityFrameworkCore()
      .AddDbContext<CmsContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.Cms.EntityFrameworkCore.SqlServer")))
      .AddSingleton<ICmsSqlHelper, CmsSqlServerHelper>()
      .AddSingleton<ISearchHelper, SqlServerSearchHelper>();
  }
}
