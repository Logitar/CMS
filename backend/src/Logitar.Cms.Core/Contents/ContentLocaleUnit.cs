using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents;

public record ContentLocaleUnit
{
  public UniqueNameUnit UniqueName { get; }

  public ContentLocaleUnit(UniqueNameUnit uniqueName)
  {
    UniqueName = uniqueName;
  }
}
