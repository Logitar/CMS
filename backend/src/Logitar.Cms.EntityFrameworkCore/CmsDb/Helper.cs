namespace Logitar.Cms.EntityFrameworkCore.CmsDb;

internal static class Helper
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
