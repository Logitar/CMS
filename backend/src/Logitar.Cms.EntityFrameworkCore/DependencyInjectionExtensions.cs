﻿using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Localization;
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
      .AddLogitarCmsInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationQuerier, ConfigurationQuerier>()
      .AddTransient<IContentQuerier, ContentQuerier>()
      .AddTransient<IContentTypeQuerier, ContentTypeQuerier>()
      .AddTransient<IFieldTypeQuerier, FieldTypeQuerier>()
      .AddTransient<ILanguageQuerier, LanguageQuerier>()
      .AddTransient<ISessionQuerier, SessionQuerier>()
      .AddTransient<IUserQuerier, UserQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<IContentRepository, ContentRepository>()
      .AddTransient<IContentTypeRepository, ContentTypeRepository>()
      .AddTransient<IFieldTypeRepository, FieldTypeRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>();
  }
}
