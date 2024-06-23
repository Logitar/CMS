using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SaveContentLocaleCommandTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _canadianEnglish;
  private readonly LanguageAggregate _canadianFrench;

  private readonly ContentTypeAggregate _blogArticleType;
  private readonly ContentTypeAggregate _blogAuthorType;

  private readonly ContentAggregate _blogArticle;
  private readonly ContentAggregate _blogAuthor;

  public SaveContentLocaleCommandTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _canadianEnglish = new(new LocaleUnit("en-CA"), isDefault: false, ActorId);
    _canadianFrench = new(new LocaleUnit("fr-CA"), isDefault: false, ActorId);

    _blogArticleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _blogAuthorType = new(new IdentifierUnit("BlogAuthor"), isInvariant: true, ActorId);

    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;
    _blogArticle = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "my-blog-article")), ActorId);
    _blogArticle.SetLocale(_canadianEnglish, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "my-blog-article-en")), ActorId);
    _blogAuthor = new(_blogAuthorType, new ContentLocaleUnit(new UniqueNameUnit(uniqueNameSettings, "my-blog-author")), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_canadianEnglish, _canadianFrench]);
    await _contentTypeRepository.SaveAsync([_blogArticleType, _blogAuthorType]);
    await _contentRepository.SaveAsync([_blogArticle, _blogAuthor]);
  }

  [Fact(DisplayName = "It should create a new content locale.")]
  public async Task It_should_create_a_new_content_locale()
  {
    SaveContentLocalePayload payload = new("my-blog-article-fr");
    SaveContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, _canadianFrench.Id.ToGuid());
    ContentItem? item = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(item);

    Assert.Equal(_blogArticle.Id.ToGuid(), item.Id);
    Assert.Equal(_blogArticle.Version + 1, item.Version);
    Assert.Equal(Contracts.Actors.Actor.System, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    ContentLocale locale = Assert.Single(item.Locales.Where(l => l.Language != null && l.Language.Id == _canadianFrench.Id.ToGuid()));
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Same(item, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(_canadianFrench.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(locale.CreatedOn, locale.UpdatedOn);
  }

  [Fact(DisplayName = "It should replace an existing content locale.")]
  public async Task It_should_replace_an_existing_content_locale()
  {
    SaveContentLocalePayload payload = new("my-blog-author-2");
    SaveContentLocaleCommand command = new(_blogAuthor.Id.ToGuid(), payload, LanguageId: null);
    ContentItem? item = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(item);

    Assert.Equal(_blogAuthor.Id.ToGuid(), item.Id);
    Assert.Equal(_blogAuthor.Version + 1, item.Version);
    Assert.Equal(Contracts.Actors.Actor.System, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    ContentLocale invariant = item.Invariant;
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Same(item, invariant.Item);
    Assert.Null(invariant.Language);
    Assert.Equal(Contracts.Actors.Actor.System, invariant.CreatedBy);
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.True(invariant.CreatedOn < invariant.UpdatedOn);
  }

  [Fact(DisplayName = "It should throw CannotCreateInvariantLocaleException when creating a locale to an invariant content.")]
  public async Task It_should_throw_CannotCreateInvariantLocaleException_when_creating_a_locale_to_an_invariant_content()
  {
    SaveContentLocalePayload payload = new(_blogAuthor.Invariant.UniqueName.Value);
    SaveContentLocaleCommand command = new(_blogAuthor.Id.ToGuid(), payload, _canadianFrench.Id.ToGuid());
    var exception = await Assert.ThrowsAsync<CannotCreateInvariantLocaleException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(_blogAuthor.Id.Value, exception.ContentId);
    Assert.Equal(_blogAuthorType.Id.Value, exception.ContentTypeId);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentAggregate blogArticle = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "other-blog-article")), ActorId);
    blogArticle.SetLocale(_canadianEnglish, blogArticle.Invariant, ActorId);
    await _contentRepository.SaveAsync(blogArticle);

    SaveContentLocalePayload payload = new(blogArticle.Invariant.UniqueName.Value);
    SaveContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, _canadianEnglish.Id.ToGuid());
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ContentAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(_canadianEnglish.Id.Value, exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveContentLocalePayload payload = new("  My new article  ");
    SaveContentLocaleCommand command = new(Guid.NewGuid(), payload, LanguageId: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal(nameof(AllowedCharactersValidator), failure.ErrorCode);
    Assert.Equal("UniqueName", failure.PropertyName);
  }

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task It_should_return_null_when_the_content_could_not_be_found()
  {
    SaveContentLocalePayload payload = new("my-new-article");
    SaveContentLocaleCommand command = new(Id: Guid.NewGuid(), payload, LanguageId: null);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }
}
