using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Web.Models.Account;

public record ChangePasswordInput
{
  public string Current { get; set; } = string.Empty;
  public string New { get; set; } = string.Empty;

  public ChangePasswordInput()
  {
  }

  public ChangePasswordInput(string current, string @new)
  {
    Current = current;
    New = @new;
  }

  public ChangePasswordPayload ToChangePasswordPayload() => new(New)
  {
    Current = Current
  };
}
