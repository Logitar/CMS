﻿using Logitar.Identity.Contracts.Users;

namespace Logitar.Cms.Contracts.Users;

public record Phone : Contact, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public Phone() : this(null, string.Empty, null, string.Empty)
  {
  }

  public Phone(string? countryCode, string number, string? extension, string e164Formatted)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    E164Formatted = e164Formatted;
  }

  public override string ToString() => E164Formatted;
}
