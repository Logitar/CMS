namespace Logitar.Cms.Contracts.Contents;

public class SaveContentLocalePayload
{
  public string UniqueName { get; set; }

  public List<FieldValue> Fields { get; set; }

  public SaveContentLocalePayload() : this(string.Empty)
  {
  }

  public SaveContentLocalePayload(string uniqueName, IEnumerable<FieldValue>? fields = null)
  {
    UniqueName = uniqueName;

    Fields = fields?.ToList() ?? [];
  }
}
