using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record ReadOnlyTextProperties : FieldTypeProperties, ITextProperties
{
  public override DataType DataType => DataType.Text;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  public ReadOnlyTextProperties() : this(TextProperties.ContentTypes.PlainText, null, null)
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
    new TextPropertiesValidator().ValidateAndThrow(this);
  }
}
