﻿namespace Logitar.Cms.Contracts.Sessions;

public record RefreshInput
{
  public string RefreshToken { get; set; } = string.Empty;

  public string? IpAddress { get; set; }
  public string? AdditionalInformation { get; set; }
}
