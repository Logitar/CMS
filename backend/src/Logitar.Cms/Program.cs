using MediatR;

namespace Logitar.Cms;

public class Program
{
  public static void Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    // TODO(fpion): initialize database

    // TODO(fpion): initialize application

    application.Run();
  }
}
