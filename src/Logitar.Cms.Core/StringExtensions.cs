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
}
