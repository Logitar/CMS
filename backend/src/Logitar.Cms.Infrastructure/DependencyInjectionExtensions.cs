using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Users;
using Logitar.Cms.Infrastructure.Actors;
using Logitar.Cms.Infrastructure.Caching;
using Logitar.Cms.Infrastructure.Queriers;
using Logitar.Cms.Infrastructure.Repositories;
using Logitar.Cms.Infrastructure.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Logitar.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddLogitarIdentityInfrastructure()
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .RemoveAll<IEventSerializer>()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddMemoryCache()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IActorService, ActorService>()
      .AddQueriers()
      .AddRepositories();
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CachingSettings.SectionKey).Get<CachingSettings>() ?? new();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<ILanguageRepository, LanguageRepository>();
  }
}
