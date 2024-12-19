namespace Logitar.Cms.Infrastructure.CmsDb;

public static class Helper // TODO(fpion): refactor
{
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
