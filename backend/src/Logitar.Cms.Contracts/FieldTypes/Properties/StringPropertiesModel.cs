namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record StringPropertiesModel : IStringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }

  public StringPropertiesModel() : this(minimumLength: null, maximumLength: null, pattern: null)
  {
  }

  public StringPropertiesModel(IStringProperties @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }

  public StringPropertiesModel(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
  }
}
