using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record PhonePayload : ContactPayload, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }

  public PhonePayload()
  {
  }

  public PhonePayload(string? countryCode, string number, string? extension, bool isVerified) : base(isVerified)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }
}
