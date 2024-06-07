using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Queriers;
using Logitar.Cms.EntityFrameworkCore.Repositories;
using Logitar.Cms.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWithEntityFrameworkCore(this IServiceCollection services)
  {
    return services
      .AddLogitarCmsWithEntityFrameworkCore()
      .AddLogitarCmsInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationQuerier, ConfigurationQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>();
  }
}
