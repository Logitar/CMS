namespace Logitar.Cms.Contracts.Contents;

public class CreateContentPayload
{
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
