using Logitar.Cms.Core.Configurations.Commands;
using Logitar.Cms.Infrastructure.Commands;
using MediatR;

namespace Logitar.Cms;

internal class Program
{
  private const string DefaultLocale = "en";
  private const string DefaultUsername = "admin";
  private const string DefaultPassword = "P@s$W0rD";

  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    string defaultLocale = configuration.GetValue<string>("CMS_LOCALE") ?? DefaultLocale;
    string username = configuration.GetValue<string>("CMS_USERNAME") ?? DefaultUsername;
    string password = configuration.GetValue<string>("CMS_PASSWORD") ?? DefaultPassword;
    await publisher.Publish(new InitializeConfigurationCommand(defaultLocale, username, password));

    application.Run();
  }
}
