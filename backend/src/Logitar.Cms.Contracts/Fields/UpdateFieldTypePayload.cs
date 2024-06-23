using Logitar.Cms.Contracts.Fields.Properties;
using Logitar.Identity.Contracts;

namespace Logitar.Cms.Contracts.Fields;

public record UpdateFieldTypePayload
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public BooleanProperties? BooleanProperties { get; set; }
  public DateTimeProperties? DateTimeProperties { get; set; }
  public NumberProperties? NumberProperties { get; set; }
  public StringProperties? StringProperties { get; set; }
  public TextProperties? TextProperties { get; set; }
}
