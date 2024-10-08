using Logitar.Identity.Domain.Passwords;

namespace Logitar.Cms.Core;

internal record PasswordMock : Password
{
  private readonly string _value;

  public PasswordMock(string value)
  {
    _value = value;
  }

  public override string Encode() => Convert.ToBase64String(Encoding.UTF8.GetBytes(_value));

  public override bool IsMatch(string password) => password == _value;
}
