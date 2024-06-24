namespace Logitar.Cms.Contracts.ContentTypes;

public record ReplaceFieldDefinitionPayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public ReplaceFieldDefinitionPayload() : this(string.Empty)
  {
  }

  public ReplaceFieldDefinitionPayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
