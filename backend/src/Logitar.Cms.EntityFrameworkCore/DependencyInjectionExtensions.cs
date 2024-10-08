using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Users;
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
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddLogitarCmsInfrastructure()
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IFieldTypeQuerier, FieldTypeQuerier>()
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IConfigurationRepository, ConfigurationRepository>()
      .AddScoped<IFieldTypeRepository, FieldTypeRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>();
  }
}
