using Logitar.Cms.Authentication;
using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Extensions;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Middlewares;
using Logitar.Cms.Settings;
using Logitar.Cms.Web;
using Logitar.Cms.Web.Constants;
using Logitar.Cms.Web.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Cms;

internal class Startup : StartupBase // TODO(fpion): reduce the size of this file
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
      //.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { }) // TODO(fpion): X-API-Key
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(_authenticationSchemes).RequireAuthenticatedUser().Build());

    CookiesSettings cookiesSettings = _configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings); // TODO(fpion): move to Logitar.Cms.Web
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    services.AddLogitarCmsWeb();

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarCmsWithEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
        healthChecks.AddDbContextCheck<CmsContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseStaticFiles();
    builder.UseSession();
    //builder.UseMiddleware<Logging>(); // TODO(fpion): Logging
    builder.UseMiddleware<RenewSession>();
    //builder.UseMiddleware<RedirectNotFound>(); // TODO(fpion): Frontend
    builder.UseAuthentication();
    builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
