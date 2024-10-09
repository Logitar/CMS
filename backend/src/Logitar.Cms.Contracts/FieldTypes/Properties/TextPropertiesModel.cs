namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record TextPropertiesModel : ITextProperties
{
  public string ContentType { get; set; }
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public TextPropertiesModel() : this(contentType: string.Empty, minimumLength: null, maximumLength: null)
  {
  }

  public TextPropertiesModel(ITextProperties text) : this(text.ContentType, text.MinimumLength, text.MaximumLength)
  {
  }

  public TextPropertiesModel(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
  }
}
