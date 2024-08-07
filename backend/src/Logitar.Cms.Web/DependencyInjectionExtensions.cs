﻿using Logitar.Cms.Core;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Authorization;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Filters;
using Logitar.Cms.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services
      .AddControllersWithViews(options =>
      {
        options.Filters.Add<ExceptionHandling>();
        options.Filters.Add<LoggingFilter>();
      })
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    string[] authenticationSchemes = Schemes.GetEnabled(configuration);

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    OpenAuthenticationSettings openAuthenticationSettings = configuration.GetSection(OpenAuthenticationSettings.SectionKey).Get<OpenAuthenticationSettings>() ?? new();
    services.AddSingleton(openAuthenticationSettings);
    services.AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build())
      .AddPolicy(Policies.User, new AuthorizationPolicyBuilder(authenticationSchemes)
        .RequireAuthenticatedUser()
        .AddRequirements(new UserAuthorizationRequirement())
        .Build());
    services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();

    CookiesSettings cookiesSettings = configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    return services
      .AddLogitarCmsCore()
      .AddSingleton<IActivityContextResolver, HttpActivityContextResolver>();
  }
}
