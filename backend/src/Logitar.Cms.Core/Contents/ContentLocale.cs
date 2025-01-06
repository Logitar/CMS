using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents;

public record ContentLocale
{
  public UniqueName UniqueName { get; }
  public DisplayName? DisplayName { get; }
  public Description? Description { get; }
  public IReadOnlyDictionary<Guid, string> FieldValues { get; }

  public ContentLocale(
    UniqueName uniqueName,
    DisplayName? displayName = null,
    Description? description = null,
    IEnumerable<KeyValuePair<Guid, string>>? fieldValues = null)
  {
    UniqueName = uniqueName;
    DisplayName = displayName;
    Description = description;

    Dictionary<Guid, string> cleanValues = [];
    if (fieldValues != null)
    {
      foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
      {
        if (string.IsNullOrWhiteSpace(fieldValue.Value))
        {
          cleanValues.Remove(fieldValue.Key);
        }
        else
        {
          cleanValues[fieldValue.Key] = fieldValue.Value.Trim();
        }
      }
    }
    FieldValues = cleanValues.AsReadOnly();
  }
}
