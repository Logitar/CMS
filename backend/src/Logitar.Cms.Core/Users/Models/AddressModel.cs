using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record AddressModel : ContactModel, IAddress
{
  public string Street { get; set; } = string.Empty;
  public string Locality { get; set; } = string.Empty;
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; } = string.Empty;
  public string Formatted { get; set; } = string.Empty;

  public AddressModel()
  {
  }

  public AddressModel(string street, string locality, string? postalCode, string? region, string country, string formatted)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
    Formatted = formatted;
  }
}
