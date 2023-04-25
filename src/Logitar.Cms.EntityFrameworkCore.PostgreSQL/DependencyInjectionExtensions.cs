using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL;

public static class DependencyInjectionExtensions
{
  private const string ConnectionStringKey = "POSTGRESQLCONNSTR_CmsContext";

  public static IServiceCollection AddLogitarCmsEntityFrameworkCorePostgreSQLStore(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>(ConnectionStringKey)
      ?? throw new InvalidOperationException($"The configuration key '{ConnectionStringKey}' could not be found.");

    return services.AddLogitarCmsEntityFrameworkCorePostgreSQLStore(connectionString);
  }

  public static IServiceCollection AddLogitarCmsEntityFrameworkCorePostgreSQLStore(this IServiceCollection services, string connectionString)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddDbContext<CmsContext>(options => options.UseNpgsql(connectionString))
      .AddEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddScoped<IEventBus, EventBus>();
  }
}
