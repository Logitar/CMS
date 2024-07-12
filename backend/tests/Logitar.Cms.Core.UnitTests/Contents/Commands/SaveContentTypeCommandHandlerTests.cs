using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveContentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentRepository> _contentRepository = new();

  private readonly SaveContentCommandHandler _handler;

  private readonly LanguageAggregate _english;
  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;

  public SaveContentCommandHandlerTests()
  {
    _handler = new(_contentRepository.Object);

    _english = new(new LocaleUnit("en"), isDefault: true);
    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
  }

  [Fact(DisplayName = "It should save the content item.")]
  public async Task It_should_save_the_content_item()
  {
    ContentLocaleUnit invariant = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "ryan-hucks"));
    ContentAggregate content = new(_blogAuthor, invariant);
    _contentRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, null, invariant.UniqueName, _cancellationToken)).ReturnsAsync(content);

    SaveContentCommand command = new(content);
    await _handler.Handle(command, _cancellationToken);

    _contentRepository.Verify(x => x.SaveAsync(content, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CmsUniqueNameAlreadyUsedException when a locale unique name is already used.")]
  public async Task It_should_throw_CmsUniqueNameAlreadyUsedException_when_a_locale_unique_name_is_already_used()
  {
    ContentLocaleUnit invariant = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "rendered-lego-acura-models"));
    ContentAggregate content = new(_blogArticle, invariant);
    content.SetLocale(_english, invariant);
    ContentAggregate other = new(_blogArticle, invariant);
    other.SetLocale(_english, invariant);
    _contentRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _english.Id, invariant.UniqueName, _cancellationToken)).ReturnsAsync(other);

    SaveContentCommand command = new(content);
    var exception = await Assert.ThrowsAsync<CmsUniqueNameAlreadyUsedException<ContentAggregate>>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
    Assert.Equal(_english.Id.Value, exception.LanguageId);
    Assert.Equal(invariant.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CmsUniqueNameAlreadyUsedException when the invariant unique name is already used.")]
  public async Task It_should_throw_CmsUniqueNameAlreadyUsedException_when_the_invariant_unique_name_is_already_used()
  {
    ContentLocaleUnit invariant = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "ryan-hucks"));
    ContentAggregate content = new(_blogAuthor, invariant);
    ContentAggregate other = new(_blogAuthor, invariant);
    _contentRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, null, invariant.UniqueName, _cancellationToken)).ReturnsAsync(other);

    SaveContentCommand command = new(content);
    var exception = await Assert.ThrowsAsync<CmsUniqueNameAlreadyUsedException<ContentAggregate>>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
    Assert.Null(exception.LanguageId);
    Assert.Equal(invariant.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
