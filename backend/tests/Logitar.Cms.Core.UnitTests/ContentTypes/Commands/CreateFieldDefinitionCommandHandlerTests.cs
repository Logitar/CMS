using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateFieldDefinitionCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IContentTypeQuerier> _contentTypeQuerier = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly CreateFieldDefinitionCommandHandler _handler;

  private readonly FieldTypeAggregate _articleTitle;
  private readonly FieldTypeAggregate _authorId;
  private readonly FieldTypeAggregate _personName;
  private readonly ContentTypeAggregate _blogArticle;
  private readonly ContentTypeAggregate _blogAuthor;

  public CreateFieldDefinitionCommandHandlerTests()
  {
    _handler = new(_contentTypeQuerier.Object, _contentTypeRepository.Object, _fieldTypeRepository.Object);

    _articleTitle = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _authorId = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "AuthorId"), new ReadOnlyStringProperties());
    _personName = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "PersonName"), new ReadOnlyStringProperties());
    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _blogAuthor = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
  }

  [Fact(DisplayName = "It should create a variant field definition.")]
  public async Task It_should_create_a_variant_field_definition()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);
    _fieldTypeRepository.Setup(x => x.LoadAsync(_articleTitle.Id, _cancellationToken)).ReturnsAsync(_articleTitle);

    CreateFieldDefinitionPayload payload = new("ArticleTitle")
    {
      FieldTypeId = _articleTitle.Id.ToGuid(),
      IsInvariant = false,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = false,
      DisplayName = " Article Title ",
      Description = "  The title of the article, will be used for the H1 tag.  ",
      Placeholder = "    "
    };
    CreateFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);

    await _handler.Handle(command, _cancellationToken);

    _contentTypeRepository.Verify(x => x.SaveAsync(_blogArticle, _cancellationToken), Times.Once);

    FieldDefinitionUnit fieldDefinition = Assert.Single(_blogArticle.FieldDefinitions.Values);
    Assert.Equal(_articleTitle.Id, fieldDefinition.FieldTypeId);
    Assert.Equal(payload.IsInvariant, fieldDefinition.IsInvariant);
    Assert.Equal(payload.IsRequired, fieldDefinition.IsRequired);
    Assert.Equal(payload.IsIndexed, fieldDefinition.IsIndexed);
    Assert.Equal(payload.IsUnique, fieldDefinition.IsUnique);
    Assert.Equal(payload.UniqueName, fieldDefinition.UniqueName.Value);
    Assert.NotNull(fieldDefinition.DisplayName);
    Assert.Equal(payload.DisplayName.Trim(), fieldDefinition.DisplayName.Value);
    Assert.NotNull(fieldDefinition.Description);
    Assert.Equal(payload.Description.Trim(), fieldDefinition.Description.Value);
    Assert.Null(fieldDefinition.Placeholder);
  }

  [Fact(DisplayName = "It should create an invariant field definition.")]
  public async Task It_should_create_an_invariant_field_definition()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogArticle.Id, _cancellationToken)).ReturnsAsync(_blogArticle);
    _fieldTypeRepository.Setup(x => x.LoadAsync(_authorId.Id, _cancellationToken)).ReturnsAsync(_authorId);

    CreateFieldDefinitionPayload payload = new("AuthorId")
    {
      FieldTypeId = _authorId.Id.ToGuid(),
      IsInvariant = true,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = false,
      DisplayName = " Author ID ",
      Description = "    ",
      Placeholder = " Paste the author ID (format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX) here. "
    };
    CreateFieldDefinitionCommand command = new(_blogArticle.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);

    await _handler.Handle(command, _cancellationToken);

    _contentTypeRepository.Verify(x => x.SaveAsync(_blogArticle, _cancellationToken), Times.Once);

    FieldDefinitionUnit fieldDefinition = Assert.Single(_blogArticle.FieldDefinitions.Values);
    Assert.Equal(_authorId.Id, fieldDefinition.FieldTypeId);
    Assert.Equal(payload.IsInvariant, fieldDefinition.IsInvariant);
    Assert.Equal(payload.IsRequired, fieldDefinition.IsRequired);
    Assert.Equal(payload.IsIndexed, fieldDefinition.IsIndexed);
    Assert.Equal(payload.IsUnique, fieldDefinition.IsUnique);
    Assert.Equal(payload.UniqueName, fieldDefinition.UniqueName.Value);
    Assert.NotNull(fieldDefinition.DisplayName);
    Assert.Equal(payload.DisplayName.Trim(), fieldDefinition.DisplayName.Value);
    Assert.Null(fieldDefinition.Description);
    Assert.NotNull(fieldDefinition.Placeholder);
    Assert.Equal(payload.Placeholder.Trim(), fieldDefinition.Placeholder.Value);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_could_not_be_found()
  {
    CreateFieldDefinitionPayload payload = new();
    CreateFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ContentTypeAggregate>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_blogAuthor.Id.Value, exception.Id);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the field type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_field_type_could_not_be_found()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);

    CreateFieldDefinitionPayload payload = new("Name")
    {
      FieldTypeId = _personName.Id.ToGuid(),
      IsInvariant = true
    };
    CreateFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<FieldTypeAggregate>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(_personName.Id.Value, exception.Id);
    Assert.Equal("FieldTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CmsUniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_CmsUniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    FieldDefinitionUnit fieldDefinition = new(_personName.Id, IsInvariant: true, IsRequired: true, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("Name"), DisplayName: null, Description: null, Placeholder: null);
    _blogAuthor.AddFieldDefinition(fieldDefinition);

    _contentTypeRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);
    _fieldTypeRepository.Setup(x => x.LoadAsync(_personName.Id, _cancellationToken)).ReturnsAsync(_personName);

    CreateFieldDefinitionPayload payload = new(fieldDefinition.UniqueName.Value)
    {
      FieldTypeId = _personName.Id.ToGuid(),
      IsInvariant = true
    };
    CreateFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), payload);
    ActivityHelper.Contextualize(command);

    var exception = await Assert.ThrowsAsync<CmsUniqueNameAlreadyUsedException<FieldDefinitionUnit>>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    _contentTypeRepository.Setup(x => x.LoadAsync(_blogAuthor.Id, _cancellationToken)).ReturnsAsync(_blogAuthor);

    CreateFieldDefinitionPayload payload = new("Name")
    {
      FieldTypeId = _personName.Id.ToGuid()
    };
    CreateFieldDefinitionCommand command = new(_blogAuthor.Id.ToGuid(), payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EqualValidator", error.ErrorCode);
    Assert.Equal("IsInvariant", error.PropertyName);
  }
}
