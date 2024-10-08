using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public record UpdateFieldTypePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public StringPropertiesModel? StringProperties { get; set; }
  public TextPropertiesModel? TextProperties { get; set; }
}
