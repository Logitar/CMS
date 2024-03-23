namespace Logitar.Cms.Contracts.Contents;

public record CreateContentPayload
{
  public Guid ContentTypeId { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateContentPayload() : this(Guid.Empty, string.Empty)
  {
  }

  public CreateContentPayload(Guid contentTypeId, string uniqueName)
  {
    ContentTypeId = contentTypeId;
    UniqueName = uniqueName;
  }
}
