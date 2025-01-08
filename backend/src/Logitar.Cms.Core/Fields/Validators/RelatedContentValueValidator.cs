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
    IReadOnlyCollection<Guid> contentIds = Parse(value);
    List<ValidationFailure> failures = new(capacity: 1 + contentIds.Count);

    if (contentIds.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value cannot be empty.", value)
      {
        ErrorCode = "NotEmptyValidator"
      };
      failures.Add(failure);
    }
    else if (contentIds.Count > 1 && !_settings.IsMultiple)
    {
      ValidationFailure failure = new(propertyName, "Exactly one value is allowed.", value)
      {
        ErrorCode = "MultipleValidator"
      };
      failures.Add(failure);
    }

    // TODO(fpion): find ContentTypeId for each ContentId
    // TODO(fpion): failure if ContentId not in results (dictionary?)
    // TODO(fpion): failure if ContentTypeId != _settings.ContentTypeId

    return new ValidationResult(failures);
  }

  private static IReadOnlyCollection<Guid> Parse(string value)
  {
    IReadOnlyCollection<Guid>? contentIds = null;
    try
    {
      contentIds = JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(value);
    }
    catch (Exception)
    {
    }

    return contentIds ?? [];
  }
}
