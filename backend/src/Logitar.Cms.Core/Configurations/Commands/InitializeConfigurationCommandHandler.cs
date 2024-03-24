using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Localization.Commands;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : INotificationHandler<InitializeConfigurationCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly ISender _sender;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository, IPasswordManager passwordManager, ISender sender, IUserManager userManager)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _passwordManager = passwordManager;
    _sender = sender;
    _userManager = userManager;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration == null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      LocaleUnit defaultLocale = new(command.DefaultLocale);
      configuration = ConfigurationAggregate.Initialize(defaultLocale, actorId);
      UserSettings userSettings = new()
      {
        UniqueName = configuration.UniqueNameSettings,
        Password = configuration.PasswordSettings,
        RequireUniqueEmail = configuration.RequireUniqueEmail
      };

      UserAggregate user = new(new UniqueNameUnit(configuration.UniqueNameSettings, command.Username), tenantId: null, actorId, userId);

      Password password = _passwordManager.ValidateAndCreate(command.Password);
      user.SetPassword(password, actorId);

      user.Locale = defaultLocale;
      user.Update(actorId);

      LanguageAggregate language = new(defaultLocale, isDefault: true, actorId);

      await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);
      await _sender.Send(new SaveLanguageCommand(language), cancellationToken);
      await _configurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
    }
    else
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
