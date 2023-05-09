namespace Logitar.Cms.Contracts.Languages;

public record Language : Aggregate
{
  public Guid Id { get; set; }

  public string Locale { get; set; } = string.Empty;
  public bool IsDefault { get; set; }
}
