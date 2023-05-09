using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts.Users;

public record Email
{
  public string Address { get; set; } = string.Empty;

  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
  public bool IsVerified { get; set; }
}
