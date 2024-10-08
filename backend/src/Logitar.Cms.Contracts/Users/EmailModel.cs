using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Contracts.Users;

public record EmailModel : Contact, IEmail
{
  public string Address { get; set; }

  public EmailModel() : this(string.Empty)
  {
  }

  public EmailModel(string address)
  {
    Address = address;
  }
}
