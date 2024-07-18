namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class TextFieldIndexEntity : FieldIndexEntity<string>
{
  public TextFieldIndexEntity(ContentLocaleEntity contentLocale, FieldDefinitionEntity fieldDefinition, string value)
    : base(contentLocale, fieldDefinition, value)
  {
  }

  private TextFieldIndexEntity() : base()
  {
  }
}
