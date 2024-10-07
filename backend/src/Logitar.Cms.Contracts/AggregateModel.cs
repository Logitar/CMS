using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms.Contracts;

public abstract class AggregateModel
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public Actor CreatedBy { get; set; } = Actor.System;
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = Actor.System;
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is AggregateModel aggregate && aggregate.GetType().Equals(GetType()) && aggregate.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{GetType()} (Id={Id})";
}
