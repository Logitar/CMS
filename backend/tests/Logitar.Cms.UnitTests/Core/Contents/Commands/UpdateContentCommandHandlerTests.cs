using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly UpdateContentCommandHandler _handler;

  private readonly ContentType _contentType = new(new Identifier("BlogArticle"), isInvariant: false);

  public UpdateContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object, _languageRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should return null when the content was not found.")]
  public async Task Given_ContentNotFound_When_Handle_Then_NullReturned()
  {
    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(Guid.NewGuid(), LanguageId: null, payload);
    ContentModel? content = await _handler.Handle(command, _cancellationToken);
    Assert.Null(content);
  }

  [Fact(DisplayName = "It should return null when the content locale was not found.")]
  public async Task Given_ContentLocaleNotFound_When_Handle_Then_NullReturned()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(content.Id.ToGuid(), language.Id.ToGuid(), payload);
    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when the language could not be found.")]
  public async Task Given_LanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(_contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    UpdateContentPayload payload = new();
    UpdateContentCommand command = new(content.Id.ToGuid(), Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateContentPayload payload = new()
    {
      UniqueName = "MyBlogArticle!",
      DisplayName = new ChangeModel<string>(RandomStringGenerator.GetString(999))
    };
    UpdateContentCommand command = new(Guid.NewGuid(), LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
  }
}
