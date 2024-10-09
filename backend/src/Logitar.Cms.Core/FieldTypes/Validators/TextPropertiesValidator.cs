using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

internal class TextPropertiesValidator : AbstractValidator<ITextProperties>
{
  private readonly HashSet<string> _contentTypes = new([MediaTypeNames.Text.Plain]);

  public TextPropertiesValidator()
  {
    RuleFor(x => x.ContentType).Must(BeAValidContentType)
      .WithErrorCode("ContentTypeValidator")
      .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}.");
    When(x => x.MinimumLength.HasValue && x.MaximumLength.HasValue, () =>
    {
      RuleFor(x => x.MinimumLength!.Value).LessThanOrEqualTo(x => x.MaximumLength!.Value);
      RuleFor(x => x.MaximumLength!.Value).GreaterThanOrEqualTo(x => x.MinimumLength!.Value);
    }).Otherwise(() =>
    {
      When(x => x.MinimumLength.HasValue, () => RuleFor(x => x.MinimumLength!.Value).GreaterThan(0));
      When(x => x.MaximumLength.HasValue, () => RuleFor(x => x.MaximumLength!.Value).GreaterThan(0));
    });
  }

  private bool BeAValidContentType(string contentType) => _contentTypes.Contains(contentType);
}
