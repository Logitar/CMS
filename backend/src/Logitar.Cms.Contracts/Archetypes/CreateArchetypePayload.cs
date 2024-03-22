namespace Logitar.Cms.Contracts.Archetypes;

public record CreateArchetypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateArchetypePayload() : this(string.Empty)
  {
  }

  public CreateArchetypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
