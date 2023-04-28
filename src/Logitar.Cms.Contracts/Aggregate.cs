using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts;

public abstract record Aggregate
{
  public Actor CreatedBy { get; private set; } = new();
  public DateTime CreatedOn { get; private set; }

  public Actor UpdatedBy { get; private set; } = new();
  public DateTime UpdatedOn { get; private set; }

  public long Version { get; private set; }
}
