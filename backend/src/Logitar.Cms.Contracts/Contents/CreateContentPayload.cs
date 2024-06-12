namespace Logitar.Cms.Contracts.Contents;

public record CreateContentPayload
{
  public Guid ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public string UniqueName { get; set; }

  public CreateContentPayload() : this(Guid.Empty, string.Empty)
  {
  }

  public CreateContentPayload(Guid contentTypeId, string uniqueName)
  {
    ContentTypeId = contentTypeId;
    UniqueName = uniqueName;
  }
}
