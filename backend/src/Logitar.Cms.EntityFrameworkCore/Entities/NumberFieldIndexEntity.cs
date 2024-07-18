namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class NumberFieldIndexEntity : FieldIndexEntity<double>
{
  public NumberFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, double value)
    : base(contentLocale, fieldDefinition, value)
  {
  }

  private NumberFieldIndexEntity() : base()
  {
  }
}
