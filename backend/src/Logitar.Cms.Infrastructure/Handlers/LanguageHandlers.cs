using Logitar.Cms.Core.Localization.Events;
using Logitar.Cms.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class LanguageHandlers : INotificationHandler<LanguageCreated>, INotificationHandler<LanguageSetDefault>, INotificationHandler<LanguageUpdated>
{
  private readonly CmsContext _context;

  public LanguageHandlers(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageCreated @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null)
    {
      language = new(@event);

      _context.Languages.Add(language);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.StreamId}' could not be found.");

    language.SetDefault(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(LanguageUpdated @event, CancellationToken cancellationToken)
  {
    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.StreamId}' could not be found.");

    language.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
