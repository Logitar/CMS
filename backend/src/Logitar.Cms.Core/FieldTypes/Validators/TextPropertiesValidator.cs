﻿using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class TextPropertiesValidator : AbstractValidator<ITextProperties>
{
  public TextPropertiesValidator()
  {
    When(x => x.MinimumLength.HasValue && x.MaximumLength.HasValue, () =>
    {
      RuleFor(x => x.MinimumLength!.Value).GreaterThan(0).LessThanOrEqualTo(x => x.MaximumLength!.Value);
      RuleFor(x => x.MaximumLength!.Value).GreaterThanOrEqualTo(x => x.MaximumLength!.Value);
    }).Otherwise(() =>
    {
      When(x => x.MinimumLength.HasValue, () => RuleFor(x => x.MinimumLength!.Value).GreaterThan(0));
      When(x => x.MaximumLength.HasValue, () => RuleFor(x => x.MaximumLength!.Value).GreaterThan(0));
    });
  }
}
