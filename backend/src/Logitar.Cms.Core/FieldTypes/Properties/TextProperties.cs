using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record TextProperties : BaseProperties, ITextProperties
{
  public override DataType DataType { get; } = DataType.Text;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  public TextProperties() : this(minimumLength: null, maximumLength: null)
  {
  }

  public TextProperties(ITextProperties @text) : this(@text.MinimumLength, @text.MaximumLength)
  {
  }

  public TextProperties(int? minimumLength, int? maximumLength)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    new TextPropertiesValidator().ValidateAndThrow(this);
  }
}
