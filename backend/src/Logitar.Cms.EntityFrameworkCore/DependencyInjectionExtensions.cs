using Logitar.Cms.Core.Archetypes;
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

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddTransient<IArchetypeQuerier, ArchetypeQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddTransient<IArchetypeRepository, ArchetypeRepository>();
  }
}
