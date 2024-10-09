using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using MediatR;
using Moq;
using ContentType = Logitar.Cms.Core.ContentTypes.ContentType;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateContentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateContentCommandHandler _handler;

  private readonly Language _english = new(new Locale("en"), isDefault: true);
  private readonly ContentType _blogArticle = new(new Identifier("BlogArticle"));
  private readonly ContentType _blogAuthor = new(new Identifier("BlogAuthor"), isInvariant: true);

  public CreateContentCommandHandlerTests()
  {
    _handler = new(_contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _languageRepository.Object, _sender.Object);

    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);
  }

  [Fact(DisplayName = "It should create a new invariant content.")]
  public async Task It_should_create_a_new_invariant_content()
  {
    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(It.IsAny<Content>(), _cancellationToken)).ReturnsAsync(model);

    CreateContentPayload payload = new("AcuraConnected")
    {
      Id = Guid.NewGuid(),
      ContentTypeId = _blogAuthor.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    command.Contextualize();

    ContentModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentCommand>(y => y.Content.Id.ToGuid() == payload.Id
        && y.Content.ContentTypeId == _blogAuthor.Id
        && y.Content.Invariant.UniqueName.Value == payload.UniqueName),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new variant content.")]
  public async Task It_should_create_a_new_variant_content()
  {
    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(It.IsAny<Content>(), _cancellationToken)).ReturnsAsync(model);

    CreateContentPayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = _english.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    command.Contextualize();

    ContentModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentCommand>(y => y.Content.ContentTypeId == _blogArticle.Id
        && y.Content.Invariant.UniqueName.Value == payload.UniqueName
        && Assert.Single(y.Content.Locales.Values).UniqueName.Value == payload.UniqueName),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_could_not_be_found()
  {
    CreateContentPayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week")
    {
      ContentTypeId = Guid.Empty
    };
    CreateContentCommand command = new(payload);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(typeof(ContentType).GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Equal(payload.ContentTypeId, exception.Id);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_could_not_be_found()
  {
    CreateContentPayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = Guid.Empty
    };
    CreateContentCommand command = new(payload);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(typeof(Language).GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Equal(payload.LanguageId, exception.Id);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ContentIdAlreadyUsedException when the content ID is already used.")]
  public async Task It_should_throw_ContentIdAlreadyUsedException_when_the_content_Id_is_already_used()
  {
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week"));
    Content content = new(_blogArticle, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    CreateContentPayload payload = new(invariant.UniqueName.Value)
    {
      Id = content.Id.ToGuid(),
      ContentTypeId = _blogArticle.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<ContentIdAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentPayload payload = new("MyBlogArticle!")
    {
      ContentTypeId = _blogAuthor.Id.ToGuid(),
      LanguageId = _english.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NullValidator" && e.PropertyName == "LanguageId");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
  }
}
