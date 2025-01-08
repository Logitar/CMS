using FluentValidation.Results;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class RelatedContentValueValidator : IFieldValueValidator
{
  private readonly IContentQuerier _contentQuerier;
  private readonly RelatedContentSettings _settings;

  public RelatedContentValueValidator(IContentQuerier contentQuerier, RelatedContentSettings settings)
  {
    _contentQuerier = contentQuerier;
    _settings = settings;
  }

  public async Task<ValidationResult> ValidateAsync(string value, string propertyName, CancellationToken cancellationToken)
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
        CustomState = new { contentIds.Count },
        ErrorCode = "MultipleValidator"
      };
      failures.Add(failure);
    }

    IReadOnlyDictionary<Guid, Guid> contentTypeIds = await _contentQuerier.FindContentTypeIdsAsync(contentIds, cancellationToken);
    Guid expectedContentTypeId = _settings.ContentTypeId.ToGuid();
    foreach (Guid contentId in contentIds)
    {
      if (!contentTypeIds.TryGetValue(contentId, out Guid contentTypeId))
      {
        ValidationFailure failure = new(propertyName, "The content could not be found.", contentId)
        {
          ErrorCode = "ContentValidator"
        };
        failures.Add(failure);
      }
      else if (contentTypeId != expectedContentTypeId)
      {
        string errorMessage = $"The content type 'Id={contentTypeId}' does not match the expected content type 'Id={expectedContentTypeId}'.";
        ValidationFailure failure = new(propertyName, errorMessage, contentId)
        {
          CustomState = new
          {
            ExpectedContentTypeId = expectedContentTypeId,
            ActualContentTypeId = contentTypeId
          },
          ErrorCode = "ContentTypeValidator"
        };
        failures.Add(failure);
      }
    }

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
