﻿using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents.Validators;

internal class UpdateContentValidator : AbstractValidator<UpdateContentPayload>
{
  public UpdateContentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(Content.UniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
