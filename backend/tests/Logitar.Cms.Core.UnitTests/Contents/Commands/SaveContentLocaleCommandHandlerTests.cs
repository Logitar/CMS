using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveContentLocaleCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly SaveContentLocaleCommandHandler _handler;

  private readonly ContentTypeAggregate _articleType;
  private readonly ContentTypeAggregate _authorType;
  private readonly ContentAggregate _article;
  private readonly ContentAggregate _author;
  private readonly LanguageAggregate _english;

  public SaveContentLocaleCommandHandlerTests()
  {
    _handler = new(_contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _languageRepository.Object, _sender.Object);

    _articleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _authorType = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _article = new(_articleType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "rendered-lego-acura-models")));
    _author = new(_authorType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "ryan-hucks")));
    _english = new(new LocaleUnit("en"), isDefault: true);
  }

  [Fact(DisplayName = "It should create a new locale.")]
  public async Task It_should_create_a_new_locale()
  {
    _contentRepository.Setup(x => x.LoadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _contentTypeRepository.Setup(x => x.LoadAsync(_articleType.Id, _cancellationToken)).ReturnsAsync(_articleType);
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);

    SaveContentLocalePayload payload = new("rendered-lego-acura-models");
    SaveContentLocaleCommand command = new(_article.Id.ToGuid(), _english.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);

    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentCommand>(y => y.Content.Equals(_article)), _cancellationToken), Times.Once);

    ContentLocaleUnit? locale = _article.TryGetLocale(_english);
    Assert.NotNull(locale);
    Assert.Equal(payload.UniqueName, locale.UniqueName.Value);
  }

  [Fact(DisplayName = "It should return null when the content cannot be found.")]
  public async Task It_should_return_null_when_the_content_cannot_be_found()
  {
    SaveContentLocalePayload payload = new("rendered-lego-acura-models");
    SaveContentLocaleCommand command = new(ContentId: Guid.NewGuid(), LanguageId: null, payload);
    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should replace an existing locale.")]
  public async Task It_should_replace_an_existing_locale()
  {
    _article.SetLocale(_english, _article.Invariant);

    _contentRepository.Setup(x => x.LoadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _contentTypeRepository.Setup(x => x.LoadAsync(_articleType.Id, _cancellationToken)).ReturnsAsync(_articleType);
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);

    SaveContentLocalePayload payload = new("rendered-lego-acura-models-2");
    SaveContentLocaleCommand command = new(_article.Id.ToGuid(), _english.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);

    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentCommand>(y => y.Content.Equals(_article)), _cancellationToken), Times.Once);

    ContentLocaleUnit? locale = _article.TryGetLocale(_english);
    Assert.NotNull(locale);
    Assert.Equal(payload.UniqueName, locale.UniqueName.Value);
  }

  [Fact(DisplayName = "It should replace the content invariant.")]
  public async Task It_should_replace_the_content_invariant()
  {
    _contentRepository.Setup(x => x.LoadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);

    SaveContentLocalePayload payload = new("rendered-lego-acura-models-2");
    SaveContentLocaleCommand command = new(_article.Id.ToGuid(), LanguageId: null, payload);
    ActivityHelper.Contextualize(command);

    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentCommand>(y => y.Content.Equals(_article)), _cancellationToken), Times.Once);

    Assert.Equal(payload.UniqueName, _article.Invariant.UniqueName.Value);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language cannot be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_cannot_be_found()
  {
    _contentRepository.Setup(x => x.LoadAsync(_article.Id, _cancellationToken)).ReturnsAsync(_article);
    _contentTypeRepository.Setup(x => x.LoadAsync(_articleType.Id, _cancellationToken)).ReturnsAsync(_articleType);

    SaveContentLocalePayload payload = new("rendered-lego-acura-models");
    SaveContentLocaleCommand command = new(_article.Id.ToGuid(), _english.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<LanguageAggregate>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_english.Id.Value, exception.Id);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CannotCreateInvariantLocaleException when creating a locale to an invariant content.")]
  public async Task It_should_throw_CannotCreateInvariantLocaleException_when_creating_a_locale_to_an_invariant_content()
  {
    _contentRepository.Setup(x => x.LoadAsync(_author.Id, _cancellationToken)).ReturnsAsync(_author);
    _contentTypeRepository.Setup(x => x.LoadAsync(_authorType.Id, _cancellationToken)).ReturnsAsync(_authorType);

    SaveContentLocalePayload payload = new("ryan-hucks");
    SaveContentLocaleCommand command = new(_author.Id.ToGuid(), _english.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<CannotCreateInvariantLocaleException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_author.Id.Value, exception.ContentId);
    Assert.Equal(_authorType.Id.Value, exception.ContentTypeId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveContentLocalePayload payload = new("rendered-lego-acura-models!");
    SaveContentLocaleCommand command = new(ContentId: Guid.NewGuid(), LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("AllowedCharactersValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }
}
