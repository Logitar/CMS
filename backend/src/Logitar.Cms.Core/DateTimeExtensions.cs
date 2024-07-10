namespace Logitar.Cms.Core;

public static class DateTimeExtensions // ISSUE: https://github.com/Logitar/CMS/issues/4
{
  public static DateTime AsUniversalTime(this DateTime value) => value.Kind switch
  {
    DateTimeKind.Local => value.ToUniversalTime(),
    DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    DateTimeKind.Utc => value,
    _ => throw new ArgumentException($"The date time kind '{value.Kind}' is not supported.", nameof(value)),
  };
}
