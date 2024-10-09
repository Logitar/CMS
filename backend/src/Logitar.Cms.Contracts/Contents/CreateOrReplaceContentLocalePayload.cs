namespace Logitar.Cms.Contracts.Contents;

public record CreateOrReplaceContentLocalePayload
{
  public string UniqueName { get; set; }

  public CreateOrReplaceContentLocalePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceContentLocalePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
