using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Cms.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class TagsValueValidatorTests
{
  private const string PropertyName = "Keywords";

  private readonly CancellationToken _cancellationToken = default;

  private readonly TagsValueValidator _validator;

  public TagsValueValidatorTests()
  {
    _validator = new();
  }

  [Theory(DisplayName = "Validation should fail when the value could not be parsed.")]
  [InlineData("")]
  [InlineData("[]")]
  [InlineData("    ")]
  [InlineData("invalid")]
  public async Task Given_NotParsed_When_ValidateAsync_Then_FailureResult(string value)
  {
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.ErrorMessage == "The value cannot be empty."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should succeed when the value is valid.")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult()
  {
    string value = @"[""hello"", ""word""]";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }
}
