namespace Logitar.Cms.Contracts.Fields.Properties;

public record TextProperties : ITextProperties
{
  public string ContentType { get; set; }
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }

  public TextProperties() : this(string.Empty)
  {
  }

  public TextProperties(string contentType)
  {
    ContentType = contentType;
  }
}
