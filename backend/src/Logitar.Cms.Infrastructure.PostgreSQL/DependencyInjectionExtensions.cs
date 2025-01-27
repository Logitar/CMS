using Logitar.Identity.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Infrastructure.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "POSTGRESQLCONNSTR_Cms";

  public static IServiceCollection AddLogitarCmsWithPostgreSQL(this IServiceCollection services, IConfiguration configuration)
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
    return services.AddLogitarCmsWithPostgreSQL(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarCmsWithPostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<CmsContext>(options => options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly("Logitar.Cms.Infrastructure.PostgreSQL")))
      .AddLogitarIdentityWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddSingleton<ICommandHelper, PostgresCommandHelper>()
      .AddSingleton<IQueryHelper, PostgreSQLQueryHelper>();
  }
}
