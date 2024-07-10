using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core;

internal record TestContext
{
  public User? User { get; set; }
}
