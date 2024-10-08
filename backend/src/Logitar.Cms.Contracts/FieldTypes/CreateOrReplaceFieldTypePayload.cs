using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public record CreateOrReplaceFieldTypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public StringPropertiesModel? StringProperties { get; set; }
  public TextPropertiesModel? TextProperties { get; set; }

  public CreateOrReplaceFieldTypePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceFieldTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
