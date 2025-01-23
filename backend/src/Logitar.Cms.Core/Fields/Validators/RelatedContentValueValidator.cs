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
    IReadOnlyCollection<Guid> contentIds;
    if (_settings.IsMultiple)
    {
      if (!TryParse(value, out contentIds))
      {
        ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized content ID array.", value)
        {
          ErrorCode = nameof(RelatedContentValueValidator)
        };
        return new ValidationResult([failure]);
      }
    }
    else if (!Guid.TryParse(value, out Guid contentId))
    {
      ValidationFailure failure = new(propertyName, "The value must be a valid content ID.", value)
      {
        ErrorCode = nameof(RelatedContentValueValidator)
      };
      return new ValidationResult([failure]);
    }
    else
    {
      contentIds = [contentId];
    }

    List<ValidationFailure> failures = new(capacity: contentIds.Count);

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

  private static bool TryParse(string value, out IReadOnlyCollection<Guid> contentIds)
  {
    IReadOnlyCollection<Guid>? deserialized = null;
    try
    {
      deserialized = JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(value);
    }
    catch (Exception)
    {
    }

    contentIds = deserialized ?? [];
    return deserialized != null;
  }
}
