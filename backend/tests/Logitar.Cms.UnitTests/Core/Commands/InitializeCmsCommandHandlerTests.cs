using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Settings;
using Logitar.Identity.Core.Users;
using Moq;

namespace Logitar.Cms.Core.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeCmsCommandHandlerTests
{
  private const string UniqueName = "admin";
  private const string Password = "admin";
  private const string DefaultLocale = "en";

  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();
  private readonly Mock<IUserSettingsResolver> _userSettingsResolver = new();

  private readonly InitializeCmsCommandHandler _handler;

  private readonly UserSettings _userSettings = new();

  public InitializeCmsCommandHandlerTests()
  {
    _handler = new(
      _languageQuerier.Object,
      _languageRepository.Object,
      _passwordManager.Object,
      _userQuerier.Object,
      _userRepository.Object,
      _userSettingsResolver.Object);

    _userSettingsResolver.Setup(x => x.Resolve()).Returns(_userSettings);
  }

  [Fact(DisplayName = "Handle: it should create a user and a language when nothing has been initialized.")]
  public async Task Given_NothingInitialized_When_Handle_Then_UserAndLanguageCreated()
  {
    _languageQuerier.Setup(x => x.FindDefaultIdAsync(_cancellationToken))
      .Throws(new InvalidOperationException("The default language entity could not be found."));

    Base64Password password = new(Password);
    _passwordManager.Setup(x => x.Create(Password)).Returns(password);

    InitializeCmsCommand command = new(UniqueName, Password, DefaultLocale);
    await _handler.Handle(command, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(
      It.Is<User>(y => y.CreatedBy.HasValue && y.CreatedBy.Value.Value == y.Id.Value
        && y.UpdatedBy.HasValue && y.UpdatedBy.Value.Value == y.Id.Value
        && y.UniqueName.Value == UniqueName && y.HasPassword),
      _cancellationToken), Times.Once);

    _languageRepository.Verify(x => x.SaveAsync(
      It.Is<Language>(y => y.CreatedBy.HasValue && y.UpdatedBy.HasValue && y.IsDefault && y.Locale.Code == DefaultLocale),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "Handle: it should not do anything when the CMS is already initialized.")]
  public async Task Given_CmsAlreadyInitialized_When_Handle_Then_DoNothing()
  {
    _userQuerier.Setup(x => x.AnyAsync(_cancellationToken)).ReturnsAsync(true);

    LanguageId languageId = LanguageId.NewId();
    _languageQuerier.Setup(x => x.FindDefaultIdAsync(_cancellationToken)).ReturnsAsync(languageId);

    InitializeCmsCommand command = new(UniqueName, Password, DefaultLocale);
    await _handler.Handle(command, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Theory(DisplayName = "Handle: it should only create a new language when the user is already initialized.")]
  [InlineData(null)]
  [InlineData("2597e842-1461-4d2b-862a-b715538a653f")]
  public async Task Given_UserAlreadyInitialized_When_Handle_Then_OnlyLanguageCreated(string? userIdValue)
  {
    _userQuerier.Setup(x => x.AnyAsync(_cancellationToken)).ReturnsAsync(true);

    _languageQuerier.Setup(x => x.FindDefaultIdAsync(_cancellationToken))
      .Throws(new InvalidOperationException("The default language entity could not be found."));

    ActorId? actorId = null;
    if (userIdValue != null)
    {
      UserId userId = new(tenantId: null, new EntityId(userIdValue));
      _userQuerier.Setup(x => x.FindIdAsync(It.Is<UniqueName>(y => y.Value == UniqueName), _cancellationToken)).ReturnsAsync(userId);

      actorId = new(userId.Value);
    }

    InitializeCmsCommand command = new(UniqueName, Password, DefaultLocale);
    await _handler.Handle(command, _cancellationToken);

    _userRepository.Verify(x => x.SaveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(
      It.Is<Language>(y => y.CreatedBy == actorId && y.UpdatedBy == actorId && y.IsDefault && y.Locale.Code == DefaultLocale),
      _cancellationToken), Times.Once);
  }
}
