namespace Logitar.Cms.Contracts.Users;

public record EmailInput
{
  public string Address { get; set; } = string.Empty;
  public bool Verify { get; set; }
}
