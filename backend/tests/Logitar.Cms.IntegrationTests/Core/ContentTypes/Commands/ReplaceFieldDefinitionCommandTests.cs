using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceFieldDefinitionCommandTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldTypeAggregate _contents;
  private readonly FieldTypeAggregate _subTitle;

  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;

  public ReplaceFieldDefinitionCommandTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;
    _contents = new(new UniqueNameUnit(uniqueNameSettings, "Contents"), new ReadOnlyTextProperties(), ActorId);
    _subTitle = new(new UniqueNameUnit(uniqueNameSettings, "SubTitle"), new ReadOnlyStringProperties(minimumLength: null, maximumLength: 128, pattern: null), ActorId);

    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _blogArticle.AddFieldDefinition(new FieldDefinitionUnit(_contents.Id, new IdentifierUnit("Contents")), ActorId);
    _blogArticle.AddFieldDefinition(new FieldDefinitionUnit(_subTitle.Id, new IdentifierUnit("SubTitle"), isIndexed: true), ActorId);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync([_contents, _subTitle]);
    await _contentTypeRepository.SaveAsync([_blogArticle, _blogAuthor]);
  }

  //[Fact(DisplayName = "It should create a new field definition.")]
  //public async Task It_should_create_a_new_field_definition()
  //{
  //  ReplaceFieldDefinitionPayload payload = new(_contents.Id.ToGuid(), "Contents");
  //  ReplaceFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
  //  ContentsType? type = await Pipeline.ExecuteAsync(command);
  //  Assert.NotNull(type);

  //  Assert.Equal(_blogArticle.Id.ToGuid(), type.Id);
  //  Assert.Equal(_blogArticle.Version + 1, type.Version);
  //  Assert.Equal(Contracts.Actors.Actor.System, type.ReplacedBy);
  //  Assert.Equal(Actor, type.UpdatedBy);
  //  Assert.True(type.ReplacedOn < type.UpdatedOn);

  //  Assert.Equal(2, type.Fields.Count);
  //  FieldDefinition field = Assert.Single(type.Fields, field => field.UniqueName == "Contents");
  //  Assert.NotEqual(Guid.Empty, field.Id);
  //  Assert.Same(type, field.ContentType);
  //  Assert.Equal(1, field.Order);
  //  Assert.Equal(_contents.Id.ToGuid(), field.FieldType.Id);
  //  Assert.Equal(payload.IsInvariant, field.IsInvariant);
  //  Assert.Equal(payload.IsRequired, field.IsRequired);
  //  Assert.Equal(payload.IsIndexed, field.IsIndexed);
  //  Assert.Equal(payload.IsUnique, field.IsUnique);
  //  Assert.Equal(payload.UniqueName, field.UniqueName);
  //  Assert.Equal(payload.DisplayName, field.DisplayName);
  //  Assert.Equal(payload.Description, field.Description);
  //  Assert.Equal(payload.Placeholder, field.Placeholder);
  //  Assert.Equal(Actor, field.ReplacedBy);
  //  Assert.Equal(Actor, field.UpdatedBy);
  //  Assert.Equal(field.ReplacedOn, field.UpdatedOn);
  //}

  [Fact(DisplayName = "It should replace a field definition without version.")]
  public async Task It_should_replace_a_field_definition_without_version()
  {
    Guid subTitleId = _blogArticle.FieldDefinitionByIds.Single(x => x.Value.UniqueName.Value == "SubTitle").Key;

    ReplaceFieldDefinitionPayload payload = new("SubTitle")
    {
      DisplayName = "  Sub-title  ",
      Description = "    ",
      Placeholder = " Enter your article sub-title "
    };
    ReplaceFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), subTitleId, payload, Version: null);
    ContentsType? type = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(type);

    Assert.Equal(_blogArticle.Id.ToGuid(), type.Id);
    Assert.Equal(_blogArticle.Version + 1, type.Version);
    Assert.Equal(Contracts.Actors.Actor.System, type.CreatedBy);
    Assert.Equal(Actor, type.UpdatedBy);
    Assert.True(type.CreatedOn < type.UpdatedOn);

    FieldDefinition field = Assert.Single(type.Fields, field => field.Id == subTitleId);
    Assert.Same(type, field.ContentType);
    Assert.Equal(1, field.Order);
    Assert.True(field.IsInvariant);
    Assert.False(field.IsRequired);
    Assert.True(field.IsIndexed);
    Assert.False(field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), field.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), field.Description);
    Assert.Equal(payload.Placeholder?.CleanTrim(), field.Placeholder);
    Assert.Equal(Contracts.Actors.Actor.System, field.CreatedBy);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.True(type.CreatedOn < field.UpdatedOn);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task It_should_return_null_when_the_content_type_could_not_be_found()
  {
    ReplaceFieldDefinitionPayload payload = new("Contents");
    ReplaceFieldDefinitionCommand command = new(Guid.NewGuid(), Guid.NewGuid(), payload, Version: null);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should throw FieldDefinitionNotFoundException when the field definition could not be found.")]
  public async Task It_should_throw_FieldDefinitionNotFoundException_when_the_field_definition_could_not_be_found()
  {
    ReplaceFieldDefinitionPayload payload = new("SubTitle");
    ReplaceFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FieldDefinitionNotFoundException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(_blogArticle.Id.Value, exception.ContentTypeId);
    Assert.Equal(command.FieldId, exception.FieldId);
    Assert.Equal("FieldId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    Guid contentsId = _blogArticle.FieldDefinitionByIds.Single(x => x.Value.UniqueName.Value == "Contents").Key;

    ReplaceFieldDefinitionPayload payload = new("SubTitle");
    ReplaceFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), contentsId, payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<FieldDefinitionUnit>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceFieldDefinitionPayload payload = new("123_JobTitle");
    ReplaceFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(IdentifierValidator), error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }
}
