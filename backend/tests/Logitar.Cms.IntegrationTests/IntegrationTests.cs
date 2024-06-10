using Bogus;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Configurations.Commands;
using Logitar.Cms.Core.Users;
using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.Commands;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected const string DefaultLocale = "en";
  protected const string UsernameString = "admin";
  protected const string PasswordString = "P@s$W0rD";

  private readonly TestContext _context = new();

  protected Faker Faker { get; } = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IRequestPipeline Pipeline { get; }
  protected CmsContext CmsContext { get; }
  protected IdentityContext IdentityContext { get; }

  public virtual Actor Actor
  {
    get
    {
      if (_context.User != null)
      {
        return new Actor(_context.User);
      }

      return Actor.System;
    }
  }
  public ActorId ActorId => new(Actor.Id);

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton(_context);
    services.AddSingleton<IActivityContextResolver, TestActivityContextResolver>();

    string connectionString;
    DatabaseProvider databaseProvider = Configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = Configuration.GetValue<string>("POSTGRESQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = Configuration.GetValue<string>("SQLCONNSTR_Cms")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarCmsWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    Pipeline = ServiceProvider.GetRequiredService<IRequestPipeline>();
    CmsContext = ServiceProvider.GetRequiredService<CmsContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    StringBuilder statement = new();
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.FieldTypes.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.Languages.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.TokenBlacklist.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.OneTimePasswords.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.Sessions.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.Users.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.ApiKeys.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.Roles.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.CustomAttributes.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(IdentityDb.Actors.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(EventDb.Events.Table).Build().Text);
    await CmsContext.Database.ExecuteSqlRawAsync(statement.ToString());

    await publisher.Publish(new InitializeConfigurationCommand(DefaultLocale, UsernameString, PasswordString));

    IUserRepository userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    UserAggregate userAggregate = Assert.Single(await userRepository.LoadAsync());
    EmailUnit email = new(Faker.Person.Email, isVerified: false);
    ActorId actorId = new(userAggregate.Id.Value);
    userAggregate.FirstName = new PersonNameUnit(Faker.Person.FirstName);
    userAggregate.LastName = new PersonNameUnit(Faker.Person.LastName);
    userAggregate.Birthdate = Faker.Person.DateOfBirth;
    userAggregate.Gender = new GenderUnit(Faker.Person.Gender.ToString());
    userAggregate.Locale = new LocaleUnit(Faker.Locale);
    userAggregate.Picture = new UrlUnit(Faker.Person.Avatar);
    userAggregate.Website = new UrlUnit($"https://{Faker.Person.Website}");
    userAggregate.Update(actorId);
    userAggregate.SetEmail(email, actorId);
    await userRepository.SaveAsync(userAggregate);

    IUserQuerier userQuerier = ServiceProvider.GetRequiredService<IUserQuerier>();
    _context.User = await userQuerier.ReadAsync(userAggregate);
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
