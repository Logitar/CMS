namespace Logitar.Cms.Contracts.Users;

public record UpdateUserInput
{
  public string? Password { get; set; }

  public EmailInput? Email { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  public string? Locale { get; set; }

  public string? Picture { get; set; }
}
