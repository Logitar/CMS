using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Contracts.Sessions;

public record Session : Aggregate
{
  public Guid Id { get; set; }

  public bool IsPersistent { get; set; }

  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }
  public bool IsActive { get; set; }

  public string? IpAddress { get; set; }
  public string? AdditionalInformation { get; set; }

  public string? RefreshToken { get; set; }

  public User User { get; set; } = new();
}
