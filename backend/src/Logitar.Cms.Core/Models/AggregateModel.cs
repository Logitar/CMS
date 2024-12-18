namespace Logitar.Cms.Core.Models;

public abstract class AggregateModel
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public ActorModel CreatedBy { get; set; } = ActorModel.System;
  public DateTime CreatedOn { get; set; }

  public ActorModel UpdatedBy { get; set; } = ActorModel.System;
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is AggregateModel aggregate && aggregate.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
