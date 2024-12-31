using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record PhoneModel : ContactModel, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
  public string E164Formatted { get; set; } = string.Empty;

  public PhoneModel()
  {
  }

  public PhoneModel(string? countryCode, string number, string? extension, string e164Formatted)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    E164Formatted = e164Formatted;
  }
}
