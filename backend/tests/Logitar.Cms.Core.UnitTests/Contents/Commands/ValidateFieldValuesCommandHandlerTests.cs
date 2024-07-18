using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.Indexing;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ValidateFieldValuesCommandHandlerTests
{
  private const string PropertyName = "Fields";

  private readonly CancellationToken _cancellationToken = default;

  private readonly Guid _articleAuthorId = Guid.Parse("ab00a18a-04b7-4503-8055-6db96b791408");
  private readonly Guid _articleTitleId = Guid.Parse("b5b2718f-0734-430d-920f-a8143484db00");
  private readonly Guid _authorSlugId = Guid.Parse("77d29a5d-0c6c-4ad1-a1c3-a91504d70ca2");

  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();
  private readonly Mock<IIndexService> _indexService = new();

  private readonly ValidateFieldValuesCommandHandler _handler;

  private readonly LanguageAggregate _english;

  private readonly FieldTypeAggregate _authorIdType;
  private readonly FieldTypeAggregate _articleTitleType;
  private readonly FieldTypeAggregate _slugType;

  private readonly ContentTypeAggregate _articleType;
  private readonly ContentTypeAggregate _authorType;

  private readonly ContentAggregate _article;
  private readonly ContentAggregate _author1;
  private readonly ContentAggregate _author2;

  public ValidateFieldValuesCommandHandlerTests()
  {
    _handler = new(_fieldTypeRepository.Object, _indexService.Object);

    _english = new(new LocaleUnit("en"), isDefault: true);

    _authorIdType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "AuthorId"), new ReadOnlyStringProperties(minimumLength: 36, maximumLength: 36, pattern: "^[0-9A-F]{8}[-]([0-9A-F]{4}[-]){3}[0-9A-F]{12}$"));
    _articleTitleType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _slugType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "Slug"), new ReadOnlyStringProperties());

    _articleType = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
    _articleType.SetFieldDefinition(_articleAuthorId, new FieldDefinitionUnit(_authorIdType.Id, IsInvariant: true, IsRequired: false, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("AuthorId"), DisplayName: null, Description: null, Placeholder: null));
    _articleType.SetFieldDefinition(_articleTitleId, new FieldDefinitionUnit(_articleTitleType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new IdentifierUnit("Title"), DisplayName: null, Description: null, Placeholder: null));

    _authorType = new(new IdentifierUnit("BlogAuthor"), isInvariant: true);
    _authorType.SetFieldDefinition(_authorSlugId, new FieldDefinitionUnit(_slugType.Id, IsInvariant: true, IsRequired: false, IsIndexed: false, IsUnique: true,
      new IdentifierUnit("Slug"), DisplayName: null, Description: null, Placeholder: null));

    ContentLocaleUnit locale = new(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "rendered-lego-acura-models"));
    _article = new(_articleType, locale);
    _article.SetLocale(_english, locale);

    _author1 = new(_authorType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "ryan-hucks")));
    _author2 = new(_authorType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "nicholas-bator")));
  }

  [Fact(DisplayName = "It should throw ValidationException when ThrowOnFailure and validation fails.")]
  public async Task It_should_throw_ValidationException_when_ThrowOnFailure_and_validation_fails()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _articleTitleType.Id), _cancellationToken))
      .ReturnsAsync([_articleTitleType]);

    ValidateFieldValuesCommand command = new(Fields: [], _articleType, _article, _english, PropertyName, ThrowOnFailure: true);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(_articleTitleId, error.AttemptedValue);
    Assert.Equal("RequiredFieldValue", error.ErrorCode);
    Assert.Equal("The field value is required.", error.ErrorMessage);
    Assert.Equal(PropertyName, error.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when a field definition does not exist.")]
  public async Task Validation_should_fail_when_a_field_definition_does_not_exist()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _articleTitleType.Id), _cancellationToken))
      .ReturnsAsync([_articleTitleType]);

    FieldValuePayload[] fields =
    [
      new(_articleTitleId, "Rendered: LEGO Acura Models"),
      new(_authorSlugId, "ryan-hucks")
    ];
    ValidateFieldValuesCommand command = new(fields, _articleType, _article, _english, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.False(result.IsValid);
    ValidationFailure error = Assert.Single(result.Errors);
    Assert.Equal(_authorSlugId, error.AttemptedValue);
    Assert.Equal("FieldDefinitionNotFound", error.ErrorCode);
    Assert.Equal("The field definition could not be found.", error.ErrorMessage);
    Assert.Equal(PropertyName, error.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when a field value conflict occurs.")]
  public async Task Validation_should_fail_when_a_field_value_conflict_occurs()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _slugType.Id), _cancellationToken))
      .ReturnsAsync([_slugType]);

    FieldValuePayload field = new(_authorSlugId, "ryan-hucks");
    _indexService.Setup(x => x.GetConflictsAsync(It.Is<IEnumerable<FieldValuePayload>>(y => y.Single().Equals(field)), _author2.Id, null, _cancellationToken))
      .ReturnsAsync([new FieldValueConflict(_authorSlugId, _author1.Id)]);

    ValidateFieldValuesCommand command = new([field], _authorType, _author2, Language: null, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.False(result.IsValid);
    ValidationFailure error = Assert.Single(result.Errors);
    Assert.Equal(_authorSlugId, error.AttemptedValue);
    Assert.Equal("FieldValueConflict", error.ErrorCode);
    Assert.Equal($"The field value is already used by the content 'Id={_author1.Id.ToGuid()}'.", error.ErrorMessage);
    Assert.Equal(PropertyName, error.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when required field values are missing.")]
  public async Task Validation_should_fail_when_required_field_values_are_missing()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _articleTitleType.Id), _cancellationToken))
      .ReturnsAsync([_articleTitleType]);

    ValidateFieldValuesCommand command = new(Fields: [], _articleType, _article, _english, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.False(result.IsValid);
    ValidationFailure error = Assert.Single(result.Errors);
    Assert.Equal(_articleTitleId, error.AttemptedValue);
    Assert.Equal("RequiredFieldValue", error.ErrorCode);
    Assert.Equal("The field value is required.", error.ErrorMessage);
    Assert.Equal(PropertyName, error.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when setting an invalid field locale.")]
  public async Task Validation_should_fail_when_setting_an_invalid_field_locale()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _articleTitleType.Id), _cancellationToken))
      .ReturnsAsync([_articleTitleType]);

    FieldValuePayload[] fields =
    [
      new(_articleAuthorId, "47b3dd76-2b03-4c0b-8caa-09ea283836ef"),
      new(_articleTitleId, "Rendered: LEGO Acura Models")
    ];
    ValidateFieldValuesCommand command = new(fields, _articleType, _article, _english, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.False(result.IsValid);
    ValidationFailure error = Assert.Single(result.Errors);
    Assert.Equal(_articleAuthorId, error.AttemptedValue);
    Assert.Equal("InvalidFieldLocale", error.ErrorCode);
    Assert.Equal("The field is invariant, but the current locale is not.", error.ErrorMessage);
    Assert.Equal(PropertyName, error.PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when there are validation errors.")]
  public async Task Validation_should_fail_when_there_are_validation_errors()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _authorIdType.Id), _cancellationToken))
      .ReturnsAsync([_authorIdType]);

    FieldValuePayload field = new(_articleAuthorId, "ryan-hucks-_________________________");
    ValidateFieldValuesCommand command = new([field], _articleType, _article, Language: null, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.False(result.IsValid);
    ValidationFailure error = Assert.Single(result.Errors);
    Assert.Equal(field.Value, error.AttemptedValue);
    Assert.Equal("RegularExpressionValidator", error.ErrorCode);
    Assert.Equal("'' is not in the correct format.", error.ErrorMessage); // ISSUE: https://github.com/Logitar/CMS/issues/3
    Assert.Equal(string.Empty, error.PropertyName); // ISSUE: https://github.com/Logitar/CMS/issues/3
  }

  [Fact(DisplayName = "Validation should succeed when there is no error.")]
  public async Task Validation_should_succeed_when_there_is_no_error()
  {
    _fieldTypeRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<FieldTypeId>>(y => y.Single() == _articleTitleType.Id), _cancellationToken))
      .ReturnsAsync([_articleTitleType]);

    FieldValuePayload field = new(_articleTitleId, "Rendered: LEGO Acura Models");
    ValidateFieldValuesCommand command = new([field], _articleType, _article, _english, PropertyName, ThrowOnFailure: false);
    ValidationResult result = await _handler.Handle(command, _cancellationToken);

    Assert.True(result.IsValid);
  }
}
