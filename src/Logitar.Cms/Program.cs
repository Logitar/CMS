
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Commands;
using MediatR;

namespace Logitar.Cms;

public class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseStaticWebAssets();

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();

    if (application.Configuration.GetValue<bool>("MigrateDatabase"))
    {
      IRequest? migrateDatabase = null;
      DatabaseProvider databaseProvider = application.Configuration.GetValue<DatabaseProvider>("DatabaseProvider");
      switch (databaseProvider)
      {
        case DatabaseProvider.EntityFrameworkCorePostgreSQL:
          migrateDatabase = new MigrateDatabase();
          break;
      }

      if (migrateDatabase != null)
      {
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(migrateDatabase);
      }
    }

    application.Run();
  }
}
