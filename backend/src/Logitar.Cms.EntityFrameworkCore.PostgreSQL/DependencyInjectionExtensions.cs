using Logitar.Identity.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "POSTGRESQLCONNSTR_Cms";

  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
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
    return services.AddLogitarCmsWithEntityFrameworkCorePostgreSQL(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddLogitarCmsWithEntityFrameworkCore()
      .AddDbContext<CmsContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitar.Cms.EntityFrameworkCore.PostgreSQL")))
      .AddSingleton<ISearchHelper, PostgresSearchHelper>();
  }
}
