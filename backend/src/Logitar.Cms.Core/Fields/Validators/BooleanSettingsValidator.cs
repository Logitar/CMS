﻿using FluentValidation;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

public class BooleanSettingsValidator : AbstractValidator<IBooleanSettings>;