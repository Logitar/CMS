using Logitar.Cms.Core.Commands;
using Logitar.Cms.Infrastructure.Commands;
using MediatR;
using Microsoft.FeatureManagement;

namespace Logitar.Cms;

internal class Program
{
  private const string DefaultUniqueName = "admin";
  private const string DefaultPassword = "admin";
  private const string DefaultLocale = "en";

  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    await startup.ConfigureAsync(application);

    IServiceScope scope = application.Services.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(FeatureFlags.MigrateDatabase))
    {
      await mediator.Send(new InitializeDatabaseCommand());
    }

    string uniqueName = configuration.GetValue<string>("CMS_USERNAME") ?? DefaultUniqueName;
    string password = configuration.GetValue<string>("CMS_PASSWORD") ?? DefaultPassword;
    string defaultLocale = configuration.GetValue<string>("CMS_LOCALE") ?? DefaultLocale;
    await mediator.Send(new InitializeCmsCommand(uniqueName, password, defaultLocale));

    application.Run();
  }
}
