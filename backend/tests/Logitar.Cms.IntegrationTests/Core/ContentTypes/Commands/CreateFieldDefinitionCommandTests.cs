using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateFieldDefinitionCommandTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _contents;
  private readonly FieldTypeAggregate _subTitle;

  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;

  public CreateFieldDefinitionCommandTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;
    _contents = new(new UniqueNameUnit(uniqueNameSettings, "Contents"), new ReadOnlyTextProperties(), ActorId);
    _subTitle = new(new UniqueNameUnit(uniqueNameSettings, "SubTitle"), new ReadOnlyStringProperties(minimumLength: null, maximumLength: 128, pattern: null), ActorId);

    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _blogArticle.AddFieldDefinition(new FieldDefinitionUnit(_subTitle.Id, new IdentifierUnit("SubTitle"), isIndexed: true), ActorId);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_contents, _subTitle]);
    await _contentTypeRepository.SaveAsync([_blogArticle, _blogAuthor]);
  }

  [Fact(DisplayName = "It should create a new field definition.")]
  public async Task It_should_create_a_new_field_definition()
  {
    CreateFieldDefinitionPayload payload = new(_contents.Id.ToGuid(), "Contents");
    CreateFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
    ContentsType? type = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(type);

    Assert.Equal(_blogArticle.Id.ToGuid(), type.Id);
    Assert.Equal(_blogArticle.Version + 1, type.Version);
    Assert.Equal(Contracts.Actors.Actor.System, type.CreatedBy);
    Assert.Equal(Actor, type.UpdatedBy);
    Assert.True(type.CreatedOn < type.UpdatedOn);

    Assert.Equal(2, type.Fields.Count);
    FieldDefinition field = Assert.Single(type.Fields, field => field.UniqueName == "Contents");
    Assert.NotEqual(Guid.Empty, field.Id);
    Assert.Same(type, field.ContentType);
    Assert.Equal(1, field.Order);
    Assert.Equal(_contents.Id.ToGuid(), field.FieldType.Id);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName, field.DisplayName);
    Assert.Equal(payload.Description, field.Description);
    Assert.Equal(payload.Placeholder, field.Placeholder);
    Assert.Equal(Actor, field.CreatedBy);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.Equal(field.CreatedOn, field.UpdatedOn);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task It_should_return_null_when_the_content_type_could_not_be_found()
  {
    CreateFieldDefinitionPayload payload = new(_contents.Id.ToGuid(), "Contents");
    CreateFieldDefinitionCommand command = new(Guid.NewGuid(), payload);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the field type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_field_type_could_not_be_found()
  {
    CreateFieldDefinitionPayload payload = new(Guid.NewGuid(), "Contents");
    CreateFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<FieldTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(new FieldTypeId(payload.FieldTypeId).Value, exception.Id);
    Assert.Equal(nameof(payload.FieldTypeId), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    CreateFieldDefinitionPayload payload = new(_contents.Id.ToGuid(), "SubTitle");
    CreateFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<FieldDefinitionUnit>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateFieldDefinitionPayload payload = new(_subTitle.Id.ToGuid(), "JobTitle")
    {
      IsInvariant = false
    };
    CreateFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EqualValidator", error.ErrorCode);
    Assert.Equal("IsInvariant", error.PropertyName);
  }
}
