using Logitar.Cms.Core.Fields.Settings;
using System.Text.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Logitar.Cms.Core.Fields.Validators;

[Trait(Traits.Category, Categories.Unit)]
public class SelectValueValidatorTests
{
  private const string PropertyName = "Category";

  private readonly CancellationToken _cancellationToken = default;

  private readonly SelectSettings _settings = new(isMultiple: false, options:
  [
    new SelectOption("linux_sysadmin"),
    new SelectOption("Software Architecture", value: "software-architecture")
  ]);
  private readonly SelectValueValidator _validator;

  public SelectValueValidatorTests()
  {
    _validator = new(_settings);
  }

  [Theory(DisplayName = "Validation should fail when the value is empty.")]
  [InlineData("[]")]
  [InlineData("[  ]")]
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
    string value = @"[""linux_sysadmin"",""software-architecture""]";
    ValidationResult result = await _validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.ErrorCode == "MultipleValidator" && e.ErrorMessage == "Exactly one value is allowed."
      && e.AttemptedValue.Equals(value) && e.PropertyName == PropertyName);
  }

  [Fact(DisplayName = "Validation should fail when values are not allowed.")]
  public async Task Given_NotAllowed_When_ValidateAsync_Then_FailureResult()
  {
    SelectSettings settings = new(isMultiple: true, _settings.Options);
    SelectValueValidator validator = new(settings);

    string value = @"[""linux_sysadmin"",""software-architecture"",""not_allowed"",""hello-world""]";
    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.False(result.IsValid);
    string customState = @"{""AllowedValues"":[""linux_sysadmin"",""software-architecture""]}";
    Assert.Contains(result.Errors, e => e.ErrorCode == "OptionValidator" && e.ErrorMessage == "The value should be one of the following: linux_sysadmin, software-architecture."
      && e.AttemptedValue.Equals("not_allowed") && e.PropertyName == PropertyName && customState == JsonSerializer.Serialize(e.CustomState, e.CustomState.GetType()));
    Assert.Contains(result.Errors, e => e.ErrorCode == "OptionValidator" && e.ErrorMessage == "The value should be one of the following: linux_sysadmin, software-architecture."
      && e.AttemptedValue.Equals("hello-world") && e.PropertyName == PropertyName && customState == JsonSerializer.Serialize(e.CustomState, e.CustomState.GetType()));
  }

  [Theory(DisplayName = "Validation should succeed when the value is valid.")]
  [InlineData("linux_sysadmin")]
  [InlineData(@"[ ""software-architecture"", ""linux_sysadmin"" ]")]
  public async Task Given_ValidValue_When_ValidateAsync_Then_SuccessResult(string value)
  {
    SelectSettings settings = new(isMultiple: true, _settings.Options);
    SelectValueValidator validator = new(settings);

    ValidationResult result = await validator.ValidateAsync(value, PropertyName, _cancellationToken);
    Assert.True(result.IsValid);
  }
}
