using Logitar.Cms.Core.Actors;

namespace Logitar.Cms.Core.Users.Models;

public abstract record ContactModel
{
  public bool IsVerified { get; set; }
  public ActorModel? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
}
