using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public record CreateFieldTypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public BooleanProperties? BooleanProperties { get; set; }
  public StringProperties? StringProperties { get; set; }
  public TextProperties? TextProperties { get; set; }

  public CreateFieldTypePayload() : this(string.Empty)
  {
  }

  public CreateFieldTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
