﻿using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts;

public abstract record Aggregate
{
  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public long Version { get; set; }
}
