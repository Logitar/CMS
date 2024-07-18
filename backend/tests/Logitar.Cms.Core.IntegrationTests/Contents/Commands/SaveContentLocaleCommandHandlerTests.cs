using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.Languages;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SaveContentLocaleCommandHandlerTests : IntegrationTests
{
  private readonly Guid _serialId = Guid.Parse("5753df17-ec04-4d60-bd25-948278f19f41");
  private readonly Guid _titleId = Guid.Parse("53901ed4-2df4-4fd9-bcb3-178f22dc8b20");
  private readonly Guid _contentId = Guid.Parse("b271716a-c57c-4992-bcd9-b1a85ffc40c2");

  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly FieldTypeAggregate _serialType;
  private readonly FieldTypeAggregate _titleType;
  private readonly FieldTypeAggregate _contentFieldType;
  private readonly ContentTypeAggregate _contentType;
  private readonly ContentAggregate _content;

  public SaveContentLocaleCommandHandlerTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _serialType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleSerial"), new ReadOnlyStringProperties(minimumLength: 10, maximumLength: 10, pattern: "^\\d{10}$"), ActorId);
    _titleType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties(minimumLength: 1, maximumLength: 100, pattern: null), ActorId);
    _contentFieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleContent"), new ReadOnlyTextProperties(TextProperties.ContentTypes.PlainText, minimumLength: 1, maximumLength: null), ActorId);

    _contentType = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _contentType.SetFieldDefinition(_serialId, new FieldDefinitionUnit(_serialType.Id, IsInvariant: true, IsRequired: true, IsIndexed: true, IsUnique: true,
      new IdentifierUnit("Serial"), DisplayName: null, Description: null, Placeholder: null), ActorId);
    _contentType.SetFieldDefinition(_titleId, new FieldDefinitionUnit(_titleType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("Title"), DisplayName: null, Description: null, Placeholder: null), ActorId);
    _contentType.SetFieldDefinition(_contentId, new FieldDefinitionUnit(_contentFieldType.Id, IsInvariant: false, IsRequired: false, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("Content"), DisplayName: null, Description: null, Placeholder: null), ActorId);

    _content = new(_contentType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "article")), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_serialType, _titleType, _contentFieldType]);
    await _contentTypeRepository.SaveAsync(_contentType);
    await _contentRepository.SaveAsync(_content);
  }

  [Fact(DisplayName = "It should save a content invariant.")]
  public async Task It_should_save_a_content_invariant()
  {
    SaveContentLocalePayload payload = new("rendered-lego-acura-models");
    FieldValue field = new(_serialId, "1826754308");
    payload.Fields.Add(field);
    SaveContentLocaleCommand command = new(_content.Id.ToGuid(), LanguageId: null, payload);
    ContentItem? content = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(content);

    Assert.Equal(_content.Id.ToGuid(), content.Id);
    Assert.Equal(_content.Version + 1, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.True(content.CreatedOn < content.UpdatedOn);

    Assert.NotEqual(default, content.Invariant.Id);
    Assert.Same(content, content.Invariant.Item);
    Assert.Null(content.Invariant.Language);
    Assert.Equal(payload.UniqueName, content.Invariant.UniqueName);
    Assert.Equal(payload.Fields, content.Invariant.Fields);
    Assert.Equal(Contracts.Actors.Actor.System, content.Invariant.CreatedBy);
    Assert.NotEqual(default, content.Invariant.CreatedOn);
    Assert.Equal(Actor, content.Invariant.UpdatedBy);
    Assert.Equal(content.Invariant.CreatedOn, content.Invariant.UpdatedOn);

    UniqueFieldIndexEntity? index = await CmsContext.UniqueFieldIndex.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(index);
    Assert.Equal(_contentType.Id.ToGuid(), index.ContentTypeUid);
    Assert.Equal(_serialType.Id.ToGuid(), index.FieldTypeUid);
    Assert.Equal(_serialId, index.FieldDefinitionUid);
    Assert.Equal(_content.Id.ToGuid(), index.ContentItemUid);
    Assert.Null(index.LanguageUid);
    Assert.Equal(field.Value, index.Value);
  }

  [Fact(DisplayName = "It should save a content locale.")]
  public async Task It_should_save_a_content_locale()
  {
    LanguageAggregate language = Assert.Single(await _languageRepository.LoadAsync());

    SaveContentLocalePayload payload = new("rendered-lego-acura-models");
    FieldValue title = new(_titleId, "Rendered: LEGO Acura Models");
    FieldValue contents = new(_contentId, "I loved my LEGO as a kid. I can vividly remember the LEGO sets I owned: the motor speedway from the 80s, a huge castle set, and a space “M-Tron” set, just to name a few. […]");
    payload.Fields.AddRange([title, contents]);
    SaveContentLocaleCommand command = new(_content.Id.ToGuid(), language.Id.ToGuid(), payload);
    ContentItem? content = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(content);

    Assert.Equal(_content.Id.ToGuid(), content.Id);
    Assert.Equal(_content.Version + 1, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.True(content.CreatedOn < content.UpdatedOn);

    ContentLocale locale = Assert.Single(content.Locales, l => l.Language != null);
    Assert.NotEqual(default, locale.Id);
    Assert.Same(content, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(language.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(payload.Fields, locale.Fields);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.NotEqual(default, locale.CreatedOn);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(locale.CreatedOn, locale.UpdatedOn);

    StringFieldIndexEntity? @string = await CmsContext.StringFieldIndex.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(@string);
    Assert.Equal(_contentType.Id.ToGuid(), @string.ContentTypeUid);
    Assert.Equal(_titleType.Id.ToGuid(), @string.FieldTypeUid);
    Assert.Equal(_titleId, @string.FieldDefinitionUid);
    Assert.Equal(_content.Id.ToGuid(), @string.ContentItemUid);
    Assert.Equal(language.Id.ToGuid(), @string.LanguageUid);
    Assert.Equal(title.Value, @string.Value);

    TextFieldIndexEntity? text = await CmsContext.TextFieldIndex.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(text);
    Assert.Equal(_contentType.Id.ToGuid(), text.ContentTypeUid);
    Assert.Equal(_contentFieldType.Id.ToGuid(), text.FieldTypeUid);
    Assert.Equal(_contentId, text.FieldDefinitionUid);
    Assert.Equal(_content.Id.ToGuid(), text.ContentItemUid);
    Assert.Equal(language.Id.ToGuid(), text.LanguageUid);
    Assert.Equal(contents.Value, text.Value);
  }
}
