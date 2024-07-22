using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceFieldTypeCommandHandlerTests : IntegrationTests
{
  private static readonly Guid _fieldId = Guid.Parse("590d72ed-8454-4c2f-9722-9e5c65c622ca");

  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _fieldType;
  private readonly ContentTypeAggregate _contentType;
  private readonly ContentAggregate _content;

  public ReplaceFieldTypeCommandHandlerTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    _fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "BlogTitle"), new ReadOnlyStringProperties());

    _contentType = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _contentType.SetFieldDefinition(_fieldId, new FieldDefinitionUnit(_fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("Title"), DisplayName: null, Description: null, Placeholder: null));

    _content = new(_contentType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "rendered-lego-acura-models"), new Dictionary<Guid, string>
    {
      [_fieldId] = "Rendered: LEGO Acura Models"
    }));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync(_fieldType);
    await _contentTypeRepository.SaveAsync(_contentType);
    await _contentRepository.SaveAsync(_content);
  }

  [Fact(DisplayName = "It should replace an existing field type.")]
  public async Task It_should_replace_an_existing_field_type()
  {
    long version = _fieldType.Version;

    _fieldType.Description = new DescriptionUnit("This is the field type for blog article titles.");
    _fieldType.Update(ActorId);
    _fieldType.SetProperties(new ReadOnlyStringProperties(minimumLength: 1, maximumLength: 100, pattern: null), ActorId);
    await _fieldTypeRepository.SaveAsync(_fieldType);

    ReplaceFieldTypePayload payload = new("ArticleTitle")
    {
      DisplayName = " Article Title ",
      Description = "    ",
      StringProperties = new StringProperties
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    ReplaceFieldTypeCommand command = new(_fieldType.Id.ToGuid(), payload, version);
    FieldType? fieldType = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(fieldType);

    Assert.Equal(_fieldType.Id.ToGuid(), fieldType.Id);
    Assert.Equal(_fieldType.Version + 1, fieldType.Version);
    Assert.Equal(Contracts.Actors.Actor.System, fieldType.CreatedBy);
    Assert.Equal(_fieldType.CreatedOn.AsUniversalTime(), fieldType.CreatedOn);
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.True(fieldType.CreatedOn < fieldType.UpdatedOn);

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(_fieldType.Description.Value, fieldType.Description);
    Assert.Equal(DataType.String, fieldType.DataType);
    Assert.Null(fieldType.BooleanProperties);
    Assert.Null(fieldType.DateTimeProperties);
    Assert.Null(fieldType.NumberProperties);
    Assert.Equal(payload.StringProperties, fieldType.StringProperties);
    Assert.Null(fieldType.TextProperties);

    StringFieldIndexEntity? index = await CmsContext.StringFieldIndex.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(index);
    Assert.Equal(fieldType.UniqueName, index.FieldTypeName);
  }
}
