using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Core.Users.Models;

public record AddressPayload : ContactPayload, IAddress
{
  public string Street { get; set; } = string.Empty;
  public string Locality { get; set; } = string.Empty;
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; } = string.Empty;

  public AddressPayload()
  {
  }

  public AddressPayload(string street, string locality, string? postalCode, string? region, string country, bool isVerified) : base(isVerified)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
  }
}
