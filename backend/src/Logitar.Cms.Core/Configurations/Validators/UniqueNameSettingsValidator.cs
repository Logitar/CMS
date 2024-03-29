﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core.Configurations.Validators;

public class UniqueNameSettingsValidator : AbstractValidator<IUniqueNameSettings>
{
  public UniqueNameSettingsValidator()
  {
    When(x => x.AllowedCharacters != null, () => RuleFor(x => x.AllowedCharacters).NotEmpty().MaximumLength(byte.MaxValue));
  }
}
