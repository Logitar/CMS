using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts;

public abstract class Aggregate
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
