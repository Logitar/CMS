using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Shared;
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
  private readonly ContentTypeAggregate _car;
  private readonly LanguageAggregate _language;

  public CreateContentCommandTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _blogArticle = new(new IdentifierUnit("BlogArticle"));
    _car = new(new IdentifierUnit("Car"), isInvariant: true);
    _language = new(new LocaleUnit("fr-CA"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_blogArticle, _car]);
    await _languageRepository.SaveAsync(_language);
  }

  [Fact(DisplayName = "It should create a new invariant content.")]
  public async Task It_should_create_a_new_invariant_content()
  {
    CreateContentPayload payload = new(_car.Id.ToGuid(), "acura-nsx");
    CreateContentCommand command = new(payload);
    ContentItem contentItem = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, contentItem.Id);
    Assert.Equal(1, contentItem.Version);
    Assert.Equal(Actor, contentItem.CreatedBy);
    Assert.Equal(Actor, contentItem.UpdatedBy);
    Assert.Equal(contentItem.CreatedOn, contentItem.UpdatedOn);

    Assert.Equal(_car.Id.ToGuid(), contentItem.ContentType.Id);

    ContentLocale invariant = contentItem.Invariant;
    Assert.Equal(payload.UniqueName.Trim(), invariant.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), invariant.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), invariant.Description);
    Assert.Same(contentItem, invariant.Item);
    Assert.Null(invariant.Language);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(contentItem.CreatedOn, invariant.CreatedOn);
    Assert.Equal(contentItem.CreatedOn, invariant.UpdatedOn);

    Assert.Empty(contentItem.Locales);

    ContentItemEntity? contentItemEntity = await CmsContext.ContentItems.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(contentItem.Id).Value);
    Assert.NotNull(contentItemEntity);
    Assert.NotNull(await CmsContext.ContentLocales.AsNoTracking().SingleOrDefaultAsync(x => x.ContentItemId == contentItemEntity.ContentItemId && x.LanguageId == null));
  }

  [Fact(DisplayName = "It should create a new localized content.")]
  public async Task It_should_create_a_new_localized_content()
  {
    CreateContentPayload payload = new(_blogArticle.Id.ToGuid(), "prolongez-lete-avec-une-acura-nsx-coupe")
    {
      LanguageId = _language.Id.ToGuid(),
      DisplayName = "  PROLONGEZ L’ÉTÉ AVEC UNE ACURA NSX COUPÉ!  ",
      Description = "    "
    };
    CreateContentCommand command = new(payload);
    ContentItem contentItem = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, contentItem.Id);
    Assert.Equal(2, contentItem.Version);
    Assert.Equal(Actor, contentItem.CreatedBy);
    Assert.Equal(Actor, contentItem.UpdatedBy);
    Assert.True(contentItem.CreatedOn < contentItem.UpdatedOn);

    Assert.Equal(_blogArticle.Id.ToGuid(), contentItem.ContentType.Id);

    ContentLocale invariant = contentItem.Invariant;
    Assert.Equal(payload.UniqueName.Trim(), invariant.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), invariant.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), invariant.Description);
    Assert.Same(contentItem, invariant.Item);
    Assert.Null(invariant.Language);
    Assert.Equal(contentItem.CreatedBy, invariant.CreatedBy);
    Assert.Equal(contentItem.CreatedBy, invariant.UpdatedBy);
    Assert.Equal(contentItem.CreatedOn, invariant.CreatedOn);
    Assert.Equal(contentItem.CreatedOn, invariant.UpdatedOn);

    ContentLocale locale = Assert.Single(contentItem.Locales);
    Assert.Equal(payload.UniqueName.Trim(), locale.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), locale.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), locale.Description);
    Assert.Same(contentItem, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(_language.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(contentItem.UpdatedBy, locale.CreatedBy);
    Assert.Equal(contentItem.UpdatedBy, locale.UpdatedBy);
    Assert.Equal(contentItem.UpdatedOn, locale.CreatedOn);
    Assert.Equal(contentItem.UpdatedOn, locale.UpdatedOn);

    ContentItemEntity? contentItemEntity = await CmsContext.ContentItems.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(contentItem.Id).Value);
    Assert.NotNull(contentItemEntity);
    Assert.NotNull(await CmsContext.ContentLocales.AsNoTracking().SingleOrDefaultAsync(x => x.ContentItemId == contentItemEntity.ContentItemId && x.LanguageId == null));
    Assert.NotNull(await CmsContext.ContentLocales.AsNoTracking().SingleOrDefaultAsync(x => x.ContentItemId == contentItemEntity.ContentItemId && x.LanguageId != null));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_could_not_be_found()
  {
    CreateContentPayload payload = new(contentTypeId: Guid.NewGuid(), uniqueName: Guid.NewGuid().ToString());
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ContentTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.ContentTypeId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.ContentTypeId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the language could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_language_could_not_be_found()
  {
    CreateContentPayload payload = new(_blogArticle.Id.ToGuid(), "prolongez-lete-avec-une-acura-nsx-coupe")
    {
      LanguageId = Guid.NewGuid()
    };
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<LanguageAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.LanguageId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.LanguageId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentAggregate content = new(_car, new ContentLocaleUnit(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), "acura-nsx")));
    await _contentRepository.SaveAsync(content);

    CreateContentPayload payload = new(_car.Id.ToGuid(), content.Invariant.UniqueName.Value);
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<Shared.UniqueNameAlreadyUsedException<ContentAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentPayload payload = new(_blogArticle.Id.ToGuid(), "prolongez-lete-avec-une-acura-nsx-coupe");
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotNullValidator", error.ErrorCode);
    Assert.Equal(nameof(payload.LanguageId), error.PropertyName);
  }
}
