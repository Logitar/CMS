using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields.Validators;

internal class FieldValueValidatorFactory : IFieldValueValidatorFactory
{
  private readonly IContentQuerier _contentQuerier;

  public FieldValueValidatorFactory(IContentQuerier contentQuerier)
  {
    _contentQuerier = contentQuerier;
  }

  public IFieldValueValidator Create(FieldType fieldType) => fieldType.DataType switch
  {
    DataType.Boolean => new BooleanValueValidator(),
    DataType.DateTime => new DateTimeValueValidator((DateTimeSettings)fieldType.Settings),
    DataType.Number => new NumberValueValidator((NumberSettings)fieldType.Settings),
    DataType.RelatedContent => new RelatedContentValueValidator(_contentQuerier, (RelatedContentSettings)fieldType.Settings),
    DataType.RichText => new RichTextValueValidator((RichTextSettings)fieldType.Settings),
    DataType.Select => new SelectValueValidator((SelectSettings)fieldType.Settings),
    DataType.String => new StringValueValidator((StringSettings)fieldType.Settings),
    DataType.Tags => new TagsValueValidator(),
    _ => throw new DataTypeNotSupportedException(fieldType.DataType),
  };
}
