namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class StringFieldIndexEntity : FieldIndexEntity<string>
{
  public const int MaximumLength = byte.MaxValue;

  public StringFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, string value)
    : base(contentLocale, fieldDefinition, value.Truncate(MaximumLength))
  {
  }

  private StringFieldIndexEntity() : base()
  {
  }
}
