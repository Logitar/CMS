namespace Logitar.Cms.Core.Users;

public record ReadOnlyEmail(string Address, bool IsVerified = false);
