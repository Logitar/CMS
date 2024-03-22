using Logitar.Cms.Core.Archetypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class Archetypes
{
  public class ArchetypeCreatedEventHandler : INotificationHandler<ArchetypeCreatedEvent>
  {
    private readonly CmsContext _context;

    public ArchetypeCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ArchetypeCreatedEvent @event, CancellationToken cancellationToken)
    {
      ArchetypeEntity? archetype = await _context.Archetypes.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (archetype == null)
      {
        archetype = new(@event);

        _context.Archetypes.Add(archetype);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ArchetypeUpdatedEventHandler : INotificationHandler<ArchetypeUpdatedEvent>
  {
    private readonly CmsContext _context;

    public ArchetypeUpdatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ArchetypeUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ArchetypeEntity archetype = await _context.Archetypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The archetype entity 'AggregateId={@event.AggregateId}' could not be found.");

      archetype.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
