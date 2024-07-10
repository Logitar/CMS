using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.Core.Logging;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Queriers;
using Logitar.Cms.EntityFrameworkCore.Repositories;
using Logitar.Cms.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCore(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .AddLogitarCmsInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  public static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationQuerier, ConfigurationQuerier>()
      .AddTransient<ILanguageQuerier, LanguageQuerier>()
      .AddTransient<ISessionQuerier, SessionQuerier>();
  }

  public static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>()
      .AddTransient<ILogRepository, LogRepository>();
  }
}
