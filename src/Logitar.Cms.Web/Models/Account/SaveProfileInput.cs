using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Web.Models.Account;

public record SaveProfileInput
{
  public EmailInput Email { get; set; } = new();

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string Locale { get; set; } = string.Empty;

  public string? Picture { get; set; }
}
