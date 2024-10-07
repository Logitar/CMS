using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateContentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateContentCommandHandler _handler;

  private readonly LanguageAggregate _english;
  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;

  public CreateContentCommandHandlerTests()
  {
    _handler = new(_contentQuerier.Object, _contentTypeRepository.Object, _languageRepository.Object, _sender.Object);

    _english = new(new LocaleUnit("en"), isDefault: true);
    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
  }

  [Fact(DisplayName = "It should create a variant content.")]
  public async Task It_should_create_a_variant_content()
  {
    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);

    CreateContentPayload payload = new("ryan-hucks")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = _english.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentCommand>(y => y.Content.ContentTypeId == _blogArticle.Id
      && y.Content.Invariant.UniqueName.Value == payload.UniqueName
    && y.Content.Locales.Count() == 1
    && y.Content.Locales[_english.Id].UniqueName.Value == payload.UniqueName
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create an invariant content.")]
  public async Task It_should_create_an_invariant_content()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);

    CreateContentPayload payload = new("ryan-hucks")
    {
      ContentTypeId = _blogAuthor.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    await _handler.Handle(command, _cancellationToken);

    _sender.Verify(x => x.Send(It.Is<SaveContentCommand>(y => y.Content.ContentTypeId == _blogAuthor.Id
      && y.Content.Invariant.UniqueName.Value == payload.UniqueName
      && y.Content.Locales.Count() == 0
    ), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type cannot be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_cannot_be_found()
  {
    CreateContentPayload payload = new("rendered-lego-acura-models")
    {
      ContentTypeId = Guid.NewGuid()
    };
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ContentTypeAggregate>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new ContentTypeId(payload.ContentTypeId).Value, exception.Id);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language cannot be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_cannot_be_found()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);

    CreateContentPayload payload = new("rendered-lego-acura-models")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = Guid.NewGuid()
    };
    CreateContentCommand command = new(payload);
    ActivityHelper.Contextualize(command);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<LanguageAggregate>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(new LanguageId(payload.LanguageId.Value).Value, exception.Id);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);

    CreateContentPayload payload = new("rendered-lego-acura-models")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = null
    };
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("LanguageId", error.PropertyName);
  }
}
