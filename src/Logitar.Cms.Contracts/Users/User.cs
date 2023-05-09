using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts.Users;

public record User : Aggregate
{
  public Guid Id { get; set; }

  public string Username { get; set; } = string.Empty;

  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }
  public bool HasPassword { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public DateTime? SignedInOn { get; set; }

  public Email? Email { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }

  public string? Locale { get; set; }

  public string? Picture { get; set; }
}
