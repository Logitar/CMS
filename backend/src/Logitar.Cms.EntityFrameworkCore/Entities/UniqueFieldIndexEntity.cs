namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class UniqueFieldIndexEntity : FieldIndexEntity<string>
{
  public const int MaximumLength = byte.MaxValue;

  public UniqueFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, string value)
    : base(contentLocale, fieldDefinition, value.Truncate(MaximumLength))
  {
  }

  private UniqueFieldIndexEntity() : base()
  {
  }
}
