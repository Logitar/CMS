namespace Logitar.Cms.Contracts.ContentTypes;

public record ReplaceContentTypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ReplaceContentTypePayload() : this(string.Empty)
  {
  }

  public ReplaceContentTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
