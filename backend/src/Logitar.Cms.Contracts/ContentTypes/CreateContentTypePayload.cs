namespace Logitar.Cms.Contracts.ContentTypes;

public record CreateContentTypePayload
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateContentTypePayload() : this(string.Empty)
  {
  }

  public CreateContentTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
