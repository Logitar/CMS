using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Core.Sessions.Models;

public class SessionModel : AggregateModel
{
  public bool IsPersistent { get; set; }
  public string? RefreshToken { get; set; }

  public bool IsActive { get; set; }
  public ActorModel? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public UserModel User { get; set; } = new();

  public SessionModel()
  {
  }

  public SessionModel(UserModel user)
  {
    User = user;
  }
}
