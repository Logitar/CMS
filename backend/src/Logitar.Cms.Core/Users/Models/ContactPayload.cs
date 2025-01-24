namespace Logitar.Cms.Core.Users.Models;

public abstract record ContactPayload
{
  public bool IsVerified { get; set; }

  protected ContactPayload()
  {
  }

  protected ContactPayload(bool isVerified)
  {
    IsVerified = isVerified;
  }
}
