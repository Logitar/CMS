using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public record ReadOnlyTextProperties : FieldTypeProperties, ITextProperties
{
  public override DataType DataType => DataType.Text;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  public ReadOnlyTextProperties() : this(MediaTypeNames.Text.Plain, null, null)
  {
  }

  public ReadOnlyTextProperties(ITextProperties text) : this(text.ContentType, text.MinimumLength, text.MaximumLength)
  {
  }

  [JsonConstructor]
  public ReadOnlyTextProperties(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
  }
}
