namespace Logitar.Cms.Web.Constants;

internal static class Cookies
{
  public const string RefreshToken = "refresh_token";

  public static readonly CookieOptions RefreshTokenOptions = new()
  {
    HttpOnly = true,
    MaxAge = TimeSpan.FromDays(7),
#if DEBUG
    SameSite = SameSiteMode.None,
#else
    SameSite = SameSiteMode.Strict,
#endif
    Secure = true
  };
}
