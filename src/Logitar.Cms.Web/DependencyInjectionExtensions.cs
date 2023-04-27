using Logitar.Cms.Core;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Authorization;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<ExceptionHandlingFilter>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services
      .AddAuthentication()
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });

    services.AddAuthorization(options =>
    {
      options.AddPolicy(Policies.User, new AuthorizationPolicyBuilder(Schemes.All)
        .RequireAuthenticatedUser()
        .AddRequirements(new UserAuthorizationRequirement())
        .Build());
    });

    services
     .AddSession(options =>
     {
       options.Cookie.SameSite = SameSiteMode.Strict;
       options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
     })
     .AddDistributedMemoryCache();

    services.AddHttpContextAccessor();
    services.AddLogitarCmsCore();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();

    return services;
  }
}
