using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateFieldDefinitionCommandHandlerTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateFieldDefinitionCommandHandlerTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
  }

  [Fact(DisplayName = "It should create a new field definition.")]
  public async Task It_should_create_a_new_field_definition()
  {
    FieldTypeAggregate articleTitle = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties(), ActorId);
    await _fieldTypeRepository.SaveAsync(articleTitle);

    ContentTypeAggregate blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    await _contentTypeRepository.SaveAsync(blogArticle);

    CreateFieldDefinitionPayload payload = new("Title")
    {
      FieldTypeId = articleTitle.Id.ToGuid(),
      IsRequired = true,
      IsIndexed = true
    };
    CreateFieldDefinitionCommand command = new(blogArticle.Id.ToGuid(), payload);
    CmsContentType result = await Pipeline.ExecuteAsync(command);

    Assert.Equal(blogArticle.Id.ToGuid(), result.Id);
    Assert.Equal(2, result.Version);
    Assert.Equal(Actor, result.CreatedBy);
    Assert.Equal(Actor, result.UpdatedBy);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    FieldDefinition fieldDefinition = Assert.Single(result.Fields);
    Assert.NotEqual(default, fieldDefinition.Id);
    Assert.Same(result, fieldDefinition.ContentType);
    Assert.Equal(0, fieldDefinition.Order);
    Assert.Equal(articleTitle.Id.ToGuid(), fieldDefinition.FieldType.Id);
    Assert.Equal(payload.IsInvariant, fieldDefinition.IsInvariant);
    Assert.Equal(payload.IsRequired, fieldDefinition.IsRequired);
    Assert.Equal(payload.IsIndexed, fieldDefinition.IsIndexed);
    Assert.Equal(payload.IsUnique, fieldDefinition.IsUnique);
    Assert.Equal(payload.UniqueName, fieldDefinition.UniqueName);
    Assert.Equal(payload.DisplayName, fieldDefinition.DisplayName);
    Assert.Equal(payload.Description, fieldDefinition.Description);
    Assert.Equal(payload.Placeholder, fieldDefinition.Placeholder);
    Assert.Equal(Actor, fieldDefinition.CreatedBy);
    Assert.Equal(result.UpdatedOn, fieldDefinition.CreatedOn);
    Assert.Equal(Actor, fieldDefinition.UpdatedBy);
    Assert.Equal(fieldDefinition.CreatedOn, fieldDefinition.UpdatedOn);
  }
}
