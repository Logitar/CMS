using FluentValidation;
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
    IReadOnlyDictionary<Guid, string>? fieldValues = null)
  {
    UniqueName = uniqueName;
    DisplayName = displayName;
    Description = description;
    FieldValues = fieldValues ?? new Dictionary<Guid, string>();
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<ContentLocale>
  {
    public Validator()
    {
      RuleForEach(x => x.FieldValues.Values).NotEmpty();
    }
  }
}
