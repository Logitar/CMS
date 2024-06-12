using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateContentCommandTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;
  private readonly LanguageAggregate _french;

  public CreateContentCommandTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true, ActorId);
    _french = new(new LocaleUnit("fr"), isDefault: false, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_blogArticle, _blogAuthor]);
    await _languageRepository.SaveAsync(_french);
  }

  [Fact(DisplayName = "It should create a new invariant content item.")]
  public async Task It_should_create_a_new_invariant_content_item()
  {
    CreateContentPayload payload = new(_blogAuthor.Id.ToGuid(), "francis-pion");
    CreateContentCommand command = new(payload);
    ContentItem item = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, item.Id);
    Assert.Equal(1, item.Version);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.Equal(item.CreatedOn, item.UpdatedOn);

    Assert.Equal(_blogAuthor.Id.ToGuid(), item.ContentType.Id);

    Assert.Equal(payload.UniqueName, item.Invariant.UniqueName);
    Assert.Null(item.Invariant.Language);
    Assert.Equal(Actor, item.Invariant.CreatedBy);
    Assert.Equal(Actor, item.Invariant.UpdatedBy);
    Assert.Equal(item.CreatedOn, item.Invariant.CreatedOn);
    Assert.Equal(item.UpdatedOn, item.Invariant.UpdatedOn);

    Assert.Empty(item.Locales);

    ContentItemEntity? entity = await CmsContext.ContentItems.AsNoTracking()
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(item.Id).Value);
    Assert.NotNull(entity);
    Assert.Empty(entity.Locales.Where(locale => locale.LanguageId.HasValue));
  }

  [Fact(DisplayName = "It should create a new variant content item.")]
  public async Task It_should_create_a_new_variant_content_item()
  {
    CreateContentPayload payload = new(_blogArticle.Id.ToGuid(), "how-to-write-integration-tests")
    {
      LanguageId = _french.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    ContentItem item = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, item.Id);
    Assert.Equal(2, item.Version);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    Assert.Equal(_blogArticle.Id.ToGuid(), item.ContentType.Id);

    Assert.Equal(payload.UniqueName, item.Invariant.UniqueName);
    Assert.Null(item.Invariant.Language);
    Assert.Equal(Actor, item.Invariant.CreatedBy);
    Assert.Equal(Actor, item.Invariant.UpdatedBy);
    Assert.Equal(item.CreatedOn, item.Invariant.CreatedOn);
    Assert.Equal(item.CreatedOn, item.Invariant.UpdatedOn);

    ContentLocale locale = Assert.Single(item.Locales);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.NotNull(locale.Language);
    Assert.Equal(_french.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(item.UpdatedOn, locale.CreatedOn);
    Assert.Equal(item.UpdatedOn, locale.UpdatedOn);

    ContentItemEntity? entity = await CmsContext.ContentItems.AsNoTracking()
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(item.Id).Value);
    Assert.NotNull(entity);
    Assert.Equal(2, entity.Locales.Count);
    Assert.Contains(entity.Locales, locale => locale.LanguageId == null);
    Assert.Contains(entity.Locales, locale => locale.LanguageId != null);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_could_not_be_found()
  {
    CreateContentPayload payload = new(contentTypeId: Guid.NewGuid(), uniqueName: "my-new-content");
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ContentTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(new AggregateId(payload.ContentTypeId).Value, exception.Id);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_could_not_be_found()
  {
    CreateContentPayload payload = new(_blogArticle.Id.ToGuid(), uniqueName: "my-new-article")
    {
      LanguageId = Guid.NewGuid()
    };
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<LanguageAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(new AggregateId(payload.LanguageId.Value).Value, exception.Id);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentLocaleUnit invariant = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "francis-pion"));
    ContentAggregate author = new(_blogAuthor, invariant, ActorId);
    await _contentRepository.SaveAsync(author);

    CreateContentPayload payload = new(_blogAuthor.Id.ToGuid(), invariant.UniqueName.Value);
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ContentAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentPayload payload = new(_blogAuthor.Id.ToGuid(), "francis-pion")
    {
      LanguageId = _french.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal("NullValidator", failure.ErrorCode);
    Assert.Equal("LanguageId", failure.PropertyName);
  }
}
