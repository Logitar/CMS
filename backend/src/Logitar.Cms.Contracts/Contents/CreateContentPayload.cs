namespace Logitar.Cms.Contracts.Contents;

public record CreateContentPayload
{
  public Guid? Id { get; set; }

  public Guid ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public string UniqueName { get; set; }

  public CreateContentPayload() : this(string.Empty)
  {
  }

  public CreateContentPayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
