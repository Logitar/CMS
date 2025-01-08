using FluentValidation.Results;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class RelatedContentValueValidator : IFieldValueValidator
{
  private readonly RelatedContentSettings _settings;

  public RelatedContentValueValidator(RelatedContentSettings settings)
  {
    _settings = settings;
  }

  public ValidationResult Validate(string value, string propertyName)
  {
    return new ValidationResult(); // TODO(fpion): implement
  }
}
