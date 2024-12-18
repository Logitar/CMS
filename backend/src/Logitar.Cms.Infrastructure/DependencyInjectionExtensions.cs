using Logitar.Cms.Core;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Infrastructure.Queriers;
using Logitar.Cms.Infrastructure.Repositories;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarCmsCore()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>()
      .AddQueriers()
      .AddRepositories();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<ILanguageQuerier, LanguageQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<ILanguageRepository, LanguageRepository>();
  }
}
