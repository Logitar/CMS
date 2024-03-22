namespace Logitar.Cms.Contracts.Archetypes;

public class Archetype : Aggregate
{
  public string Identifier { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Archetype() : this(string.Empty)
  {
  }

  public Archetype(string identifier)
  {
    Identifier = identifier;
  }

  public override string ToString() => $"{DisplayName ?? Identifier} | {base.ToString()}";
}
