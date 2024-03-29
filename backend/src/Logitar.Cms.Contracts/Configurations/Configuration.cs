﻿namespace Logitar.Cms.Contracts.Configurations;

public class Configuration : Aggregate
{
  public string Secret { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }

  public LoggingSettings LoggingSettings { get; set; } = new();

  public Configuration() : this(string.Empty)
  {
  }

  public Configuration(string secret)
  {
    Secret = secret;
  }
}
