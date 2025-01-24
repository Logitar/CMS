using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record EmailPayload : ContactPayload, IEmail
{
  public string Address { get; set; } = string.Empty;

  public EmailPayload()
  {
  }

  public EmailPayload(string address, bool isVerified) : base(isVerified)
  {
    Address = address;
  }
}
