namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class BooleanFieldIndexEntity : FieldIndexEntity<bool>
{
  public BooleanFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, bool value)
    : base(contentLocale, fieldDefinition, value)
  {
  }

  private BooleanFieldIndexEntity() : base()
  {
  }
}
