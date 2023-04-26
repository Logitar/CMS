using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;

internal abstract class AggregateEntity
{
  protected AggregateEntity()
  {
  }
  protected AggregateEntity(DomainEvent e)
  {
    AggregateId = e.AggregateId.Value;
    SetVersion(e);

    CreatedById = e.ActorId.Value;
    CreatedOn = e.OccurredOn;
  }

  public string AggregateId { get; private set; } = string.Empty;
  public long Version { get; private set; }

  public string CreatedById { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedById { get; private set; }
  public DateTime? UpdatedOn { get; private set; }

  protected void SetVersion(DomainEvent e) => Version = e.Version;

  protected void Update(DomainEvent e)
  {
    SetVersion(e);

    UpdatedById = e.ActorId.Value;
    UpdatedOn = e.OccurredOn;
  }
}
