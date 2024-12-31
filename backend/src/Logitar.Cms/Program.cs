using Logitar.Cms.Infrastructure.Commands;
using MediatR;
using Microsoft.FeatureManagement;

namespace Logitar.Cms;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    await startup.ConfigureAsync(application);

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(FeatureFlags.MigrateDatabase))
    {
      IServiceScope scope = application.Services.CreateScope();
      IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      await mediator.Publish(new InitializeDatabaseCommand());
    }

    application.Run();
  }
}
