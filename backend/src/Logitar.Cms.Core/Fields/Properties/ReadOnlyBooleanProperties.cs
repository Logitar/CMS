using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public record ReadOnlyBooleanProperties : FieldTypeProperties, IBooleanProperties
{
  public override DataType DataType => DataType.Boolean;

  public ReadOnlyBooleanProperties()
  {
    new BooleanPropertiesValidator().ValidateAndThrow(this);
  }

  public ReadOnlyBooleanProperties(IBooleanProperties _) : this()
  {
  }
}
