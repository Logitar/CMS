using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts.Users;

public abstract record Contact
{
  public bool IsVerified { get; set; }
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
}
