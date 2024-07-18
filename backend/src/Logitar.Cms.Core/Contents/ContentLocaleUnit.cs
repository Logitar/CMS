using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public record ContentLocaleUnit
{
  public UniqueNameUnit UniqueName { get; }
  public IReadOnlyDictionary<Guid, string> FieldValues { get; }

  public ContentLocaleUnit(UniqueNameUnit uniqueName) : this(uniqueName, new Dictionary<Guid, string>().AsReadOnly())
  {
  }

  [JsonConstructor]
  public ContentLocaleUnit(UniqueNameUnit uniqueName, IReadOnlyDictionary<Guid, string> fieldValues)
  {
    UniqueName = uniqueName;
    FieldValues = fieldValues;
  }
}
