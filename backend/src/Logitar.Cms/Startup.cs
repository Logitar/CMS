using Logitar.Cms.Authentication;
using Logitar.Cms.Authorization;
using Logitar.Cms.Constants;
using Logitar.Cms.Core;
using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Extensions;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Middlewares;
using Logitar.Cms.Settings;
using Logitar.Cms.Web;
using Logitar.Cms.Web.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Cms;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly string[] _authenticationSchemes;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _authenticationSchemes = Schemes.GetEnabled(configuration);
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    AuthorizationPolicy portalActorPolicy = new AuthorizationPolicyBuilder(_authenticationSchemes)
      .RequireAuthenticatedUser()
      .AddRequirements(new UserAuthorizationRequirement())
      .Build();
    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(portalActorPolicy)
      .AddPolicy(Policies.User, portalActorPolicy);
    services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();

    BearerSettings bearerSettings = _configuration.GetSection("Bearer").Get<BearerSettings>() ?? new();
    services.AddSingleton(bearerSettings);
    services.AddTransient<IBearerService, BearerService>();

    CookiesSettings cookiesSettings = _configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddTransient<IRequestPipeline, HttpRequestPipeline>();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    // TODO(fpion): GraphQL
    services.AddLogitarCmsWeb();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    // TODO(fpion): GraphQL

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    builder.UseMiddleware<RenewSession>();
    builder.UseMiddleware<RedirectNotFound>();
    builder.UseAuthentication();
    builder.UseAuthorization();

    // TODO(fpion): GraphQL

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
