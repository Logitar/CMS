using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record EmailModel : ContactModel, IEmail
{
  public string Address { get; set; } = string.Empty;

  public EmailModel()
  {
  }

  public EmailModel(string address)
  {
    Address = address;
  }
}
