using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public record ReplaceFieldTypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public BooleanProperties? BooleanProperties { get; set; }
  public DateTimeProperties? DateTimeProperties { get; set; }
  public NumberProperties? NumberProperties { get; set; }
  public StringProperties? StringProperties { get; set; }
  public TextProperties? TextProperties { get; set; }

  public ReplaceFieldTypePayload() : this(string.Empty)
  {
  }

  public ReplaceFieldTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
