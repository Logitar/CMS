using Logitar.Identity.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Infrastructure.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "SQLCONNSTR_Cms";

  public static IServiceCollection AddLogitarCmsWithSqlServer(this IServiceCollection services, IConfiguration configuration)
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
    return services.AddLogitarCmsWithSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarCmsWithSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<CmsContext>(options => options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("Logitar.Cms.Infrastructure.SqlServer")))
      .AddLogitarIdentityWithEntityFrameworkCoreSqlServer(connectionString)
      .AddSingleton<ICommandHelper, SqlServerCommandHelper>()
      .AddSingleton<IQueryHelper, SqlServerQueryHelper>();
  }
}
