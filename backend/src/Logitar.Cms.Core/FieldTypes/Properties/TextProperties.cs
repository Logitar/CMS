using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record TextProperties : BaseProperties, ITextProperties
{
  public override DataType DataType { get; } = DataType.Text;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  public TextProperties(ITextProperties text) : this(text.ContentType, text.MinimumLength, text.MaximumLength)
  {
  }

  public TextProperties(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    new TextPropertiesValidator().ValidateAndThrow(this);
  }
}
