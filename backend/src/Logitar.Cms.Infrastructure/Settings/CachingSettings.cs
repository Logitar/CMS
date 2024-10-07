namespace Logitar.Cms.Infrastructure.Settings;

public record CachingSettings
{
  public const string SectionKey = "Caching";

  public TimeSpan? ActorLifetime { get; set; }
}
