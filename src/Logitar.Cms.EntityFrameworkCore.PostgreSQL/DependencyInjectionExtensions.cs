using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.Core.Users;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Converters;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;
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
    EventSerializer.Instance.RegisterConverter(new Pbkdf2Converter());

    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddDbContext<CmsContext>(options => options.UseNpgsql(connectionString))
      .AddEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddRepositories()
      .AddScoped<IEventBus, EventBus>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>()
      .AddScoped<IUserRepository, UserRepository>();
  }
}
