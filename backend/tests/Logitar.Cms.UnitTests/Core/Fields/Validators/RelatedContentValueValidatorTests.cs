using Bogus;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Identity.Core;
using Moq;
using System.Reflection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Cms.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class RelatedContentValueValidatorTests
{
  private const string PropertyName = "Author";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IContentQuerier> _contentQuerier = new();

  private readonly ContentType _articleType = new(new Identifier("BlogArticle"), isInvariant: false);
  private readonly ContentType _authorType = new(new Identifier("BlogAuthor"));
  private readonly ContentType _categoryType = new(new Identifier("BlogCategory"));

  private readonly Content _article;
  private readonly Content _author1;
  private readonly Content _author2;
  private readonly Content _category;

  private readonly RelatedContentSettings _settings;
  private readonly RelatedContentValueValidator _validator;

  public RelatedContentValueValidatorTests()
  {
    _article = new(_articleType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "my-blog-article"), new DisplayName("My Blog Article!")));
    _author1 = new(_authorType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName), new DisplayName(_faker.Person.FullName)));
    _author2 = new(_authorType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Internet.UserName()), new DisplayName(_faker.Name.FullName())));
    _category = new(_categoryType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "software-architecture")));

    _settings = new(_authorType.Id, isMultiple: false);
    _validator = new(_contentQuerier.Object, _settings);
  }

  [Fact(DisplayName = "Validation should fail when contents are from another content type.")]
  public async Task Given_OtherContentType_When_ValidateAsync_Then_FailureResult()
  {
    RelatedContentSettings settings = new(_settings.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, settings);

    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(new Guid[] { _article.Id.ToGuid(), _author1.Id.ToGuid(), _author2.Id.ToGuid(), _category.Id.ToGuid() })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid>
      {
        [_article.Id.ToGuid()] = _article.ContentTypeId.ToGuid(),
        [_author1.Id.ToGuid()] = _author1.ContentTypeId.ToGuid(),
        [_author2.Id.ToGuid()] = _author2.ContentTypeId.ToGuid(),
        [_category.Id.ToGuid()] = _category.ContentTypeId.ToGuid()
      });

    string value = $@"[ ""{_article.Id.ToGuid()}"", ""{_author1.Id.ToGuid()}"", ""{_author2.Id.ToGuid()}"", ""{_category.Id.ToGuid()}"" ]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.ErrorMessage == $"The content type 'Id={_articleType.Id.ToGuid()}' does not match the expected content type 'Id={_authorType.Id.ToGuid()}'."
      && e.AttemptedValue.Equals(_article.Id.ToGuid()) && e.PropertyName == PropertyName
      && HasProperty(e.CustomState, "ExpectedContentTypeId", _authorType.Id.ToGuid())
      && HasProperty(e.CustomState, "ActualContentTypeId", _articleType.Id.ToGuid()));
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.ErrorMessage == $"The content type 'Id={_categoryType.Id.ToGuid()}' does not match the expected content type 'Id={_authorType.Id.ToGuid()}'."
      && e.AttemptedValue.Equals(_category.Id.ToGuid()) && e.PropertyName == PropertyName
      && HasProperty(e.CustomState, "ExpectedContentTypeId", _authorType.Id.ToGuid())
      && HasProperty(e.CustomState, "ActualContentTypeId", _categoryType.Id.ToGuid()));
  }

  [Fact(DisplayName = "Validation should fail when contents could not be found.")]
  public async Task Given_NotFound_When_ValidateAsync_Then_FailureResult()
  {
    RelatedContentSettings settings = new(_settings.ContentTypeId, isMultiple: true);
    RelatedContentValueValidator validator = new(_contentQuerier.Object, settings);

    Guid missing1 = Guid.NewGuid();
    Guid missing2 = Guid.NewGuid();
    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(new Guid[] { _author1.Id.ToGuid(), missing1, missing2 })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid> { [_author1.Id.ToGuid()] = _author1.ContentTypeId.ToGuid() });

    string value = $@"[ ""{_author1.Id.ToGuid()}"", ""{missing1}"", ""{missing2}"" ]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentValidator" && e.ErrorMessage == "The content could not be found."
      && e.AttemptedValue.Equals(missing1) && e.PropertyName == PropertyName);
    Assert.Contains(result.Errors, e => e.ErrorCode == "ContentValidator" && e.ErrorMessage == "The content could not be found."
      && e.AttemptedValue.Equals(missing2) && e.PropertyName == PropertyName);
  }

  [Theory(DisplayName = "Validation should fail when the value is empty.")]
  [InlineData("[]")]
  [InlineData("[  ]")]
  [InlineData(@"[""invalid""]")]
  public async Task Given_Empty_When_ValidateAsync_Then_FailureResult(string value)
  {
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.ErrorMessage == "The value cannot be empty."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when there are too many values.")]
  public async Task Given_NotMultiple_When_ValidateAsync_Then_FailureResult()
  {
    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(new Guid[] { _author1.Id.ToGuid(), _author2.Id.ToGuid() })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid>
      {
        [_author1.Id.ToGuid()] = _author1.ContentTypeId.ToGuid(),
        [_author2.Id.ToGuid()] = _author2.ContentTypeId.ToGuid()
      });

    string value = $@"[""{_author1.Id.ToGuid()}"",""{_author2.Id.ToGuid()}""]";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MultipleValidator" && e.ErrorMessage == "Exactly one value is allowed."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Theory(DisplayName = "Validation should succeed when the value is valid.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult(bool isMultiple)
  {
    string value = _author1.Id.ToGuid().ToString();

    RelatedContentValueValidator validator = _validator;
    if (isMultiple)
    {
      RelatedContentSettings settings = new(_settings.ContentTypeId, isMultiple: true);
      validator = new(_contentQuerier.Object, settings);

      value = $@"[ ""{_author1.Id.ToGuid()}"", ""{_author2.Id.ToGuid()}"" ]";
    }

    _contentQuerier.Setup(x => x.FindContentTypeIdsAsync(
      It.Is<IEnumerable<Guid>>(y => y.SequenceEqual(isMultiple ? new Guid[] { _author1.Id.ToGuid(), _author2.Id.ToGuid() } : new Guid[] { _author1.Id.ToGuid() })),
      _cancellationToken)).ReturnsAsync(new Dictionary<Guid, Guid>
      {
        [_author1.Id.ToGuid()] = _author1.ContentTypeId.ToGuid(),
        [_author2.Id.ToGuid()] = _author2.ContentTypeId.ToGuid()
      });

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }

  private static bool HasProperty(object instance, string propertyName, object? propertyValue)
  {
    PropertyInfo? property = instance.GetType().GetProperty(propertyName);
    Assert.NotNull(property);

    object? value = property.GetValue(instance);
    return propertyValue == null ? value == null : propertyValue.Equals(value);
  }
}
