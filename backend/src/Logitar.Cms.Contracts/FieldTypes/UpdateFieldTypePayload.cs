using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public record UpdateFieldTypePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public BooleanProperties? BooleanProperties { get; set; }
  public DateTimeProperties? DateTimeProperties { get; set; }
  public NumberProperties? NumberProperties { get; set; }
  public StringProperties? StringProperties { get; set; }
  public TextProperties? TextProperties { get; set; }
}
