using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record ReadOnlyBooleanProperties : FieldTypeProperties, IBooleanProperties
{
  public override DataType DataType => DataType.Boolean;

  [JsonConstructor]
  public ReadOnlyBooleanProperties()
  {
    new BooleanPropertiesValidator().ValidateAndThrow(this);
  }

  public ReadOnlyBooleanProperties(IBooleanProperties _) : this()
  {
  }
}
