namespace Logitar.Cms.Contracts.Contents;

public class SaveContentLocalePayload
{
  public string UniqueName { get; set; }

  public SaveContentLocalePayload() : this(string.Empty)
  {
  }

  public SaveContentLocalePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
