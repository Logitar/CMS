using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Contracts.Sessions;

public class SessionModel : AggregateModel
{
  public string? RefreshToken { get; set; }
  public bool IsPersistent { get; set; }

  public bool IsActive { get; set; }
  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public UserModel User { get; set; }

  public SessionModel() : this(new UserModel())
  {
  }

  public SessionModel(UserModel user)
  {
    User = user;

    CustomAttributes = [];
  }
}
