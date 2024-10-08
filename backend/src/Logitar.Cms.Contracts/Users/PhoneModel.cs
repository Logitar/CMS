using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Contracts.Users;

public record PhoneModel : Contact, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public PhoneModel() : this(string.Empty, string.Empty)
  {
  }

  public PhoneModel(string number, string e164Formatted)
  {
    Number = number;
    E164Formatted = e164Formatted;
  }
}
