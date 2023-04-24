namespace Logitar.Cms.Schema;

public record GraphQLSettings
{
  public bool EnableMetrics { get; init; }
  public bool ExposeExceptionDetails { get; init; }
}
