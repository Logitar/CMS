using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Commands;

public record InitializeCmsCommand(string UniqueName, string Password, string DefaultLocale) : IRequest;

internal class InitializeCmsCommandHandler : IRequestHandler<InitializeCmsCommand>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;
  private readonly IUserSettingsResolver _userSettingsResolver;

  public InitializeCmsCommandHandler(
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IPasswordManager passwordManager,
    IUserQuerier userQuerier,
    IUserRepository userRepository,
    IUserSettingsResolver userSettingsResolver)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _passwordManager = passwordManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
    _userSettingsResolver = userSettingsResolver;
  }

  public async Task Handle(InitializeCmsCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _userSettingsResolver.Resolve();
    UniqueName uniqueName = new(userSettings.UniqueName, command.UniqueName);

    UserId? userId;
    ActorId? actorId = null;

    if (!await _userQuerier.AnyAsync(cancellationToken))
    {
      userId = UserId.NewId();
      actorId = new(userId.Value.Value);
      User user = new(uniqueName, actorId, userId);

      Password password = _passwordManager.ValidateAndCreate(command.Password);
      user.SetPassword(password, actorId);

      await _userRepository.SaveAsync(user, cancellationToken);
    }

    LanguageId? languageId = null;
    try
    {
      languageId = await _languageQuerier.FindDefaultIdAsync(cancellationToken);
    }
    catch (InvalidOperationException)
    {
    }
    if (!languageId.HasValue)
    {
      if (!actorId.HasValue)
      {
        userId = await _userQuerier.FindIdAsync(uniqueName, cancellationToken);
        if (userId.HasValue)
        {
          actorId = new(userId.Value.Value);
        }
      }

      Locale locale = new(command.DefaultLocale);
      languageId = LanguageId.NewId();
      Language language = new(locale, isDefault: true, actorId, languageId);
      await _languageRepository.SaveAsync(language, cancellationToken);
    }
  }
}
