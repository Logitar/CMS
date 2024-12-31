using Logitar.Identity.Core.Passwords;
using System.Text;

namespace Logitar.Cms;

internal record Base64Password : Password
{
  public const string Key = "BASE64";

  private readonly string _base64;

  public Base64Password(string password)
  {
    _base64 = ToBase64(password);
  }

  private static string ToBase64(string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

  public override string Encode() => string.Join(Separator, Key, _base64);

  public override bool IsMatch(string password) => _base64.Equals(ToBase64(password));
}
