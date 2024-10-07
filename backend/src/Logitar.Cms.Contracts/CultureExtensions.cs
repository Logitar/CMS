namespace Logitar.Cms.Contracts;

public static class CultureExtensions
{
  public static RegionInfo? GetRegion(this CultureInfo culture)
  {
    try
    {
      return new RegionInfo(culture.LCID);
    }
    catch (Exception)
    {
      return null;
    }
  }
}
