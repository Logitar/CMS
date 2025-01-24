namespace Logitar.Cms.Core.Users.Models;

public record ChangePasswordPayload
{
  public string? Current { get; set; }
  public string New { get; set; } = string.Empty;

  public ChangePasswordPayload()
  {
  }

  public ChangePasswordPayload(string newPassword)
  {
    New = newPassword;
  }
}
