namespace Logitar.Cms.Contracts.Archetypes;

public class Archetype : Aggregate
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Archetype() : this(string.Empty)
  {
  }

  public Archetype(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
