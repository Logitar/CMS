using Logitar.Cms.Core;
using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Commands;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.Commands;
using Logitar.Cms.Infrastructure.SqlServer;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using CmsDb = Logitar.Cms.Infrastructure.CmsDb;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms;

public abstract class IntegrationTests : IAsyncLifetime
{
  private const string DefaultUniqueName = "admin";
  private const string DefaultPassword = "P@s$W0rD";
  private const string DefaultLocale = "en";

  private readonly DatabaseProvider _databaseProvider;

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IMediator Mediator { get; }
  protected CmsContext CmsContext { get; }

  private readonly TestContext _context = new();
  protected ActorModel Actor => _context.Actor ?? new();
  protected ActorId ActorId => new(Actor.Id);

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();
    _databaseProvider = Configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.SqlServer;

    ServiceCollection services = new();
    services.AddSingleton(Configuration);

    services.AddLogitarCmsCore();
    services.AddLogitarCmsInfrastructure();
    switch (_databaseProvider)
    {
      case DatabaseProvider.SqlServer:
        string connectionString = Configuration.GetValue<string>("SQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarCmsWithSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    services.AddSingleton<IApplicationContext, TestApplicationContext>();
    services.AddSingleton(_context);
    ServiceProvider = services.BuildServiceProvider();

    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    CmsContext = ServiceProvider.GetRequiredService<CmsContext>();
  }

  public virtual async Task InitializeAsync()
  {
    await Mediator.Send(new InitializeDatabaseCommand());

    StringBuilder sql = new();
    TableId[] tables =
    [
      CmsDb.Contents.Table,
      CmsDb.ContentTypes.Table,
      CmsDb.FieldTypes.Table,
      CmsDb.Languages.Table,
      IdentityDb.Users.Table,
      IdentityDb.Actors.Table,
      EventDb.Streams.Table
    ];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      sql.Append(command.Text).Append(';').AppendLine();
    }
    await CmsContext.Database.ExecuteSqlRawAsync(sql.ToString());

    string uniqueName = Configuration.GetValue<string>("CMS_USERNAME") ?? DefaultUniqueName;
    string password = Configuration.GetValue<string>("CMS_PASSWORD") ?? DefaultPassword;
    string defaultLocale = Configuration.GetValue<string>("CMS_LOCALE") ?? DefaultLocale;
    await Mediator.Send(new InitializeCmsCommand(uniqueName, password, defaultLocale));

    IdentityContext identityContext = ServiceProvider.GetRequiredService<IdentityContext>();
    ActorEntity actor = await identityContext.Actors.AsNoTracking().SingleAsync();
    _context.Actor = Mapper.ToActor(actor);
  }
  protected IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.SqlServer => new SqlServerDeleteBuilder(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
