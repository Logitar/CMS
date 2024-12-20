﻿namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews() // TODO(fpion): Error Handling, Logging
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    return services;
  }
}
