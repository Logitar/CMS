using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateContentLocaleCommandTests : IntegrationTests
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

  public UpdateContentLocaleCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task It_should_return_null_when_the_content_could_not_be_found()
  {
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "my-new-article"
    };
    UpdateContentLocaleCommand command = new(Id: Guid.NewGuid(), payload, LanguageId: null);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_could_not_be_found()
  {
    LanguageAggregate mexicanSpanish = new(new LocaleUnit("es-MX"), isDefault: false, ActorId);
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "mi-blog-articulo"
    };
    UpdateContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, mexicanSpanish.Id.ToGuid());
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<LanguageAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(mexicanSpanish.Id.Value, exception.Id);
    Assert.Equal(nameof(command.LanguageId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ContentLocaleNotFoundException when the content locale could not be found.")]
  public async Task It_should_throw_ContentLocaleNotFoundException_when_the_content_locale_could_not_be_found()
  {
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "my-blog-article-fr"
    };
    UpdateContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, _canadianFrench.Id.ToGuid());
    var exception = await Assert.ThrowsAsync<ContentLocaleNotFoundException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(_blogArticle.Id.Value, exception.ContentId);
    Assert.Equal(_canadianFrench.Id.Value, exception.LanguageId);
    Assert.Equal(nameof(command.LanguageId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentAggregate blogArticle = new(_blogArticleType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "other-blog-article")), ActorId);
    blogArticle.SetLocale(_canadianEnglish, blogArticle.Invariant, ActorId);
    await _contentRepository.SaveAsync(blogArticle);

    UpdateContentLocalePayload payload = new()
    {
      UniqueName = blogArticle.Invariant.UniqueName.Value
    };
    UpdateContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, _canadianEnglish.Id.ToGuid());
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ContentAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(_canadianEnglish.Id.Value, exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "  My new article  "
    };
    UpdateContentLocaleCommand command = new(Guid.NewGuid(), payload, LanguageId: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal(nameof(AllowedCharactersValidator), failure.ErrorCode);
    Assert.Equal("UniqueName", failure.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing locale.")]
  public async Task It_should_update_an_existing_locale()
  {
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "my-blog-article-en-ca"
    };
    UpdateContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, _canadianEnglish.Id.ToGuid());
    ContentItem? item = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(item);

    Assert.Equal(_blogArticle.Id.ToGuid(), item.Id);
    Assert.Equal(_blogArticle.Version + 1, item.Version);
    Assert.Equal(Contracts.Actors.Actor.System, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    ContentLocale locale = Assert.Single(item.Locales.Where(l => l.Language != null && l.Language.Id == _canadianEnglish.Id.ToGuid()));
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Same(item, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(_canadianEnglish.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(Contracts.Actors.Actor.System, locale.CreatedBy);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.True(locale.CreatedOn < locale.UpdatedOn);
  }

  [Fact(DisplayName = "It should update the invariant.")]
  public async Task It_should_update_the_invariant()
  {
    UpdateContentLocalePayload payload = new()
    {
      UniqueName = "my-blog-article-2"
    };
    UpdateContentLocaleCommand command = new(_blogArticle.Id.ToGuid(), payload, LanguageId: null);
    ContentItem? item = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(item);

    Assert.Equal(_blogArticle.Id.ToGuid(), item.Id);
    Assert.Equal(_blogArticle.Version + 1, item.Version);
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
}
