namespace Logitar.Cms.Infrastructure.Settings;

internal record CachingSettings
{
  public const string SectionKey = "Caching";

  public TimeSpan ActorLifetime { get; set; } = TimeSpan.FromMinutes(15);
}
