using Logitar.Cms.Core.Archetypes.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ArchetypeEntity : AggregateEntity
{
  public int ArchetypeId { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public ArchetypeEntity(ArchetypeCreatedEvent @event) : base(@event)
  {
    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ArchetypeEntity() : base()
  {
  }

  public void Update(ArchetypeUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
