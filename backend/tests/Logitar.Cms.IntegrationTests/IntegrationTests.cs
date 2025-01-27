using Bogus;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Commands;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.Commands;
using Logitar.Cms.Infrastructure.PostgreSQL;
using Logitar.Cms.Infrastructure.SqlServer;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CmsDb = Logitar.Cms.Infrastructure.CmsDb;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IMediator Mediator { get; }
  protected CmsContext CmsContext { get; }
  protected IdentityContext IdentityContext { get; }

  private readonly TestContext _context = new();
  protected ActorModel Actor => _context.Actor ?? new();
  protected ActorId ActorId => new(Actor.Id);
  protected Faker Faker { get; } = new();

  protected string UniqueName => Configuration.GetValue<string>("CMS_USERNAME") ?? "admin";
  protected string Password => Configuration.GetValue<string>("CMS_PASSWORD") ?? "P@s$W0rD";
  protected string LocaleCode => Configuration.GetValue<string>("CMS_LOCALE") ?? Faker.Locale;

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
    string connectionString;
    switch (_databaseProvider)
    {
      case DatabaseProvider.PostgreSQL:
        connectionString = Configuration.GetValue<string>("POSTGRESQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarCmsWithPostgreSQL(connectionString);
        break;
      case DatabaseProvider.SqlServer:
        connectionString = Configuration.GetValue<string>("SQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
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
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
  }

  public virtual async Task InitializeAsync()
  {
    await Mediator.Send(new InitializeDatabaseCommand());

    StringBuilder sql = new();
    TableId[] tables =
    [
      CmsDb.UniqueIndex.Table,
      CmsDb.FieldIndex.Table,
      CmsDb.PublishedContents.Table,
      CmsDb.Contents.Table,
      CmsDb.ContentTypes.Table,
      CmsDb.FieldTypes.Table,
      CmsDb.Languages.Table,
      IdentityDb.Sessions.Table,
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

    await Mediator.Send(new InitializeCmsCommand(UniqueName, Password, LocaleCode));

    ActorEntity actor = await IdentityContext.Actors.AsNoTracking().SingleAsync();
    _context.Actor = Mapper.ToActor(actor);
  }
  protected IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.PostgreSQL => new PostgresDeleteBuilder(table),
    DatabaseProvider.SqlServer => new SqlServerDeleteBuilder(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
