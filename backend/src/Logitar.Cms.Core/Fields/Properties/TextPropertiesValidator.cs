using FluentValidation;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public class TextPropertiesValidator : AbstractValidator<ITextProperties>
{
  private static readonly HashSet<string> _contentTypes = new([MediaTypeNames.Text.Plain]);

  public TextPropertiesValidator()
  {
    RuleFor(x => x.ContentType).NotEmpty().Must(_contentTypes.Contains)
      .WithErrorCode("ContentTypeValidator")
      .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", _contentTypes)}.");
    RuleFor(x => x.MinimumLength).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MaximumLength);
    RuleFor(x => x.MaximumLength).GreaterThan(0);
  }
}
