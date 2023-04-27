using System.Globalization;

namespace Logitar.Cms.Core;

internal static class StringExtensions
{
  public static CultureInfo GetCultureInfo(this string name, string paramName)
  {
    try
    {
      return CultureInfo.GetCultureInfo(name);
    }
    catch (CultureNotFoundException innerException)
    {
      throw new InvalidLocaleException(name, paramName, innerException);
    }
  }

  public static Uri? GetUri(this string url, string paramName)
  {
    try
    {
      return string.IsNullOrWhiteSpace(url) ? null : new(url);
    }
    catch (Exception innerException)
    {
      throw new InvalidUrlException(url, paramName, innerException);
    }
  }
}
