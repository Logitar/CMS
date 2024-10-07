namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class DateTimeFieldIndexEntity : FieldIndexEntity<DateTime>
{
  public DateTimeFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, DateTime value)
    : base(contentLocale, fieldDefinition, value)
  {
  }

  private DateTimeFieldIndexEntity() : base()
  {
  }
}
