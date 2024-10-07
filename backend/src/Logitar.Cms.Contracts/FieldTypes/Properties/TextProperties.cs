namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record TextProperties : ITextProperties
{
  public static class ContentTypes
  {
    public const string PlainText = MediaTypeNames.Text.Plain;
  }

  public string ContentType { get; set; }
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public TextProperties() : this(ContentTypes.PlainText, minimumLength: null, maximumLength: null)
  {
  }

  public TextProperties(ITextProperties text) : this(text.ContentType, text.MinimumLength, text.MaximumLength)
  {
  }

  public TextProperties(string contentType, int? minimumLength, int? maximumLength)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
  }
}
