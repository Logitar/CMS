namespace Logitar.Cms.Core.Fields.Validators;

public interface IFieldValueValidatorFactory
{
  IFieldValueValidator Create(FieldType fieldType);
}
