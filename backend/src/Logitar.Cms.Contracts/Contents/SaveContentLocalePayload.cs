namespace Logitar.Cms.Contracts.Contents;

public class SaveContentLocalePayload
{
  public string UniqueName { get; set; }

  public List<FieldValuePayload> Fields { get; set; }

  public SaveContentLocalePayload() : this(string.Empty)
  {
  }

  public SaveContentLocalePayload(string uniqueName, IEnumerable<FieldValuePayload>? fields = null)
  {
    UniqueName = uniqueName;

    Fields = fields?.ToList() ?? [];
  }
}
