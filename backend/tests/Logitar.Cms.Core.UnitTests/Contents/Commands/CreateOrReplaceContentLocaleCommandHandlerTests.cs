using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using MediatR;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceContentLocaleCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceContentLocaleCommandHandler _handler;

  private readonly Language _english = new(new Locale("en"), isDefault: true);
  private readonly Language _french = new(new Locale("fr"), isDefault: true);
  private readonly ContentType _articleType = new(new Identifier("BlogArticle"));
  private readonly ContentType _authorType = new(new Identifier("BlogAuthor"), isInvariant: true);
  private readonly Content _articleContent;
  private readonly Content _authorContent;

  public CreateOrReplaceContentLocaleCommandHandlerTests()
  {
    _handler = new(_contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _languageRepository.Object, _sender.Object);

    ContentLocale article = new(new UniqueName(Content.UniqueNameSettings, "new-blog-article"));
    _articleContent = new(_articleType, article);
    _articleContent.SetLocale(_english, article);

    ContentLocale author = new(new UniqueName(Content.UniqueNameSettings, "acura-connected"));
    _authorContent = new(_authorType, author);

    _languageRepository.Setup(x => x.LoadAsync(_english.Id, _cancellationToken)).ReturnsAsync(_english);
    _languageRepository.Setup(x => x.LoadAsync(_french.Id, _cancellationToken)).ReturnsAsync(_french);
    _contentTypeRepository.Setup(x => x.LoadAsync(_articleType.Id, _cancellationToken)).ReturnsAsync(_articleType);
    _contentTypeRepository.Setup(x => x.LoadAsync(_authorType.Id, _cancellationToken)).ReturnsAsync(_authorType);
    _contentRepository.Setup(x => x.LoadAsync(_articleContent.Id, _cancellationToken)).ReturnsAsync(_articleContent);
    _contentRepository.Setup(x => x.LoadAsync(_authorContent.Id, _cancellationToken)).ReturnsAsync(_authorContent);
  }

  [Fact(DisplayName = "It should create a new locale.")]
  public async Task It_should_create_a_new_locale()
  {
    CreateOrReplaceContentLocalePayload payload = new("lacura-adx-donne-un-apercu-des-caracteristiques-haut-de-gamme-du-tout-nouveau-vus-dentree-de-gamme-de-la-marque-acura");
    CreateOrReplaceContentLocaleCommand command = new(_articleContent.Id.ToGuid(), _french.Id.ToGuid(), payload);
    command.Contextualize();

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(_articleContent, _cancellationToken)).ReturnsAsync(model);

    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentCommand>(y => y.Content.Equals(_articleContent) && y.Content.GetLocale(_french).UniqueName.Value == payload.UniqueName),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing locale.")]
  public async Task It_should_replace_an_existing_locale()
  {
    CreateOrReplaceContentLocalePayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week");
    CreateOrReplaceContentLocaleCommand command = new(_articleContent.Id.ToGuid(), _english.Id.ToGuid(), payload);
    command.Contextualize();

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(_articleContent, _cancellationToken)).ReturnsAsync(model);

    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentCommand>(y => y.Content.Equals(_articleContent) && y.Content.GetLocale(_english).UniqueName.Value == payload.UniqueName),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace the invariant.")]
  public async Task It_should_replace_the_invariant()
  {
    CreateOrReplaceContentLocalePayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week");
    CreateOrReplaceContentLocaleCommand command = new(_articleContent.Id.ToGuid(), LanguageId: null, payload);
    command.Contextualize();

    ContentModel model = new();
    _contentQuerier.Setup(x => x.ReadAsync(_articleContent, _cancellationToken)).ReturnsAsync(model);

    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _sender.Verify(x => x.Send(
      It.Is<SaveContentCommand>(y => y.Content.Equals(_articleContent) && y.Content.Invariant.UniqueName.Value == payload.UniqueName),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task It_should_return_null_when_the_content_could_not_be_found()
  {
    CreateOrReplaceContentLocalePayload payload = new("acura-integra-type-s-hrc-prototype-debuts-at-monterey-car-week");
    CreateOrReplaceContentLocaleCommand command = new(Guid.NewGuid(), LanguageId: null, payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_could_not_be_found()
  {
    CreateOrReplaceContentLocalePayload payload = new("lacura-adx-donne-un-apercu-des-caracteristiques-haut-de-gamme-du-tout-nouveau-vus-dentree-de-gamme-de-la-marque-acura");
    CreateOrReplaceContentLocaleCommand command = new(_articleContent.Id.ToGuid(), Guid.NewGuid(), payload);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(typeof(Language).GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Equal(command.LanguageId, exception.Id);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CannotCreateInvariantLocaleException when adding a locale to an invariant content.")]
  public async Task It_should_throw_CannotCreateInvariantLocaleException_when_adding_a_locale_to_an_invariant_content()
  {
    CreateOrReplaceContentLocalePayload payload = new("AcuraConnected");
    CreateOrReplaceContentLocaleCommand command = new(_authorContent.Id.ToGuid(), _english.Id.ToGuid(), payload);
    command.Contextualize();

    var exception = await Assert.ThrowsAsync<CannotCreateInvariantLocaleException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_authorType.Id.ToGuid(), exception.ContentTypeId);
    Assert.Equal(_authorContent.Id.ToGuid(), exception.ContentId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceContentLocalePayload payload = new("MyBlogArticle!");
    CreateOrReplaceContentLocaleCommand command = new(Guid.NewGuid(), LanguageId: null, payload);

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("AllowedCharactersValidator", error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }
}
