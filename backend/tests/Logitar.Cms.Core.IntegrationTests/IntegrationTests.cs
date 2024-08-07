﻿using Bogus;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations.Commands;
using Logitar.Cms.EntityFrameworkCore;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL;
using Logitar.Cms.EntityFrameworkCore.SqlServer;
using Logitar.Cms.Infrastructure;
using Logitar.Cms.Infrastructure.Commands;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected const string DefaultLocale = "en";
  protected const string UsernameString = "admin";
  protected const string PasswordString = "P@s$W0rD";

  protected Faker Faker { get; } = new();
  private readonly TestContext _context = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IRequestPipeline Pipeline { get; }
  protected CmsContext CmsContext { get; }
  protected EventContext EventContext { get; }
  protected IdentityContext IdentityContext { get; }

  protected Actor Actor
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
  protected ActorId ActorId => new(Actor.Id);

  protected IntegrationTests() : base()
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
        services.AddLogitarCmsWithEntityFrameworkCorePostgreSQL(connectionString);
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
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    IdentityContext = ServiceProvider.GetRequiredService<IdentityContext>();
  }

  public virtual async Task InitializeAsync()
  {
    ISender sender = ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new InitializeDatabaseCommand());

    StringBuilder statement = new();
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.UniqueFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.TextFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.StringFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.NumberFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.DateTimeFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.BooleanFieldIndex.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.ContentItems.Table).Build().Text);
    statement.AppendLine(SqlServerDeleteBuilder.From(CmsDb.ContentTypes.Table).Build().Text);
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

    await sender.Send(new InitializeConfigurationCommand(DefaultLocale, UsernameString, PasswordString));

    IUserRepository userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
    UserAggregate user = Assert.Single(await userRepository.LoadAsync());
    Assert.True(user.HasPassword);
    Assert.NotNull(user.Locale);

    Actor actor = new(user.UniqueName.Value)
    {
      Id = user.Id.ToGuid(),
      Type = Contracts.Actors.ActorType.User
    };
    _context.User = new User(user.UniqueName.Value)
    {
      Id = user.Id.ToGuid(),
      Version = user.Version,
      CreatedBy = actor,
      CreatedOn = user.CreatedOn.AsUniversalTime(),
      UpdatedBy = actor,
      UpdatedOn = user.UpdatedOn.AsUniversalTime(),
      HasPassword = true,
      PasswordChangedBy = actor,
      PasswordChangedOn = user.UpdatedOn.AsUniversalTime(),
      Locale = new Locale(user.Locale.Code)
    };
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
