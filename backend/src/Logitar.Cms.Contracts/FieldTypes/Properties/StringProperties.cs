namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record StringProperties : IStringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }

  public StringProperties() : this(minimumLength: null, maximumLength: null, pattern: null)
  {
  }

  public StringProperties(IStringProperties @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }

  public StringProperties(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
  }
}
