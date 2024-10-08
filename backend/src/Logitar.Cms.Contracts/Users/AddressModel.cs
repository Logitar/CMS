using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Contracts.Users;

public record AddressModel : Contact, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public string Formatted { get; set; }

  public AddressModel() : this(string.Empty, string.Empty, string.Empty, string.Empty)
  {
  }

  public AddressModel(string street, string locality, string country, string formatted)
  {
    Street = street;
    Locality = locality;
    Country = country;
    Formatted = formatted;
  }
}
