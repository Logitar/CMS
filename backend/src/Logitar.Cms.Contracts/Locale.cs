﻿namespace Logitar.Cms.Contracts;

public class Locale
{
  public int Id { get; set; }
  public string Code { get; set; }
  public string DisplayName { get; set; }
  public string EnglishName { get; set; }
  public string NativeName { get; set; }

  public Locale() : this(string.Empty)
  {
  }

  public Locale(string cultureName) : this(CultureInfo.GetCultureInfo(cultureName.Trim()))
  {
  }

  public Locale(CultureInfo culture) : this(culture.LCID, culture.Name, culture.DisplayName, culture.EnglishName, culture.NativeName)
  {
  }

  public Locale(int id, string code, string displayName, string englishName, string nativeName)
  {
    Id = id;
    Code = code;
    DisplayName = displayName;
    EnglishName = englishName;
    NativeName = nativeName;
  }

  public static Locale? TryCreate(string? code) => string.IsNullOrWhiteSpace(code) ? null : new(code);

  public override bool Equals(object? obj) => obj is Locale locale && locale.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => DisplayName;
}
