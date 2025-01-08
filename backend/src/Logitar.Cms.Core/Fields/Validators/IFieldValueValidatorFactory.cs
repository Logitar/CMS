namespace Logitar.Cms.Core.Fields.Validators;

internal interface IFieldValueValidatorFactory
{
  IFieldValueValidator Create(FieldType fieldType);
}
