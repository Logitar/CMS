namespace Logitar.Cms.Contracts.ContentTypes;

public record CreateOrReplaceContentTypePayload
{
  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateOrReplaceContentTypePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceContentTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
