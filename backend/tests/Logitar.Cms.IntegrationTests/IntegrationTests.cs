using Bogus;
using Logitar.Cms.Core;
using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.Commands;
using Logitar.Data.SqlServer;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected Faker Faker { get; } = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IRequestPipeline Pipeline { get; }
  protected CmsContext CmsContext { get; }

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton<IRequestPipeline, TestRequestPipeline>();
    services.AddSingleton(TestContext.Random(Faker));

    DatabaseProvider databaseProvider = Configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        string connectionString = Configuration.GetValue<string>("SQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    Pipeline = ServiceProvider.GetRequiredService<IRequestPipeline>();
    CmsContext = ServiceProvider.GetRequiredService<CmsContext>();
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    StringBuilder statement = new();
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.ContentTypes.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(EventDb.Events.Table).Build().Text);
    await CmsContext.Database.ExecuteSqlRawAsync(statement.ToString());
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
