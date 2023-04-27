using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Users;

public record ReadOnlyEmail(string Address, bool IsVerified = false)
{
  public static ReadOnlyEmail? From(EmailInput? input)
  {
    return input == null ? null : new ReadOnlyEmail(input.Address.Trim(), input.Verify);
  }
}
