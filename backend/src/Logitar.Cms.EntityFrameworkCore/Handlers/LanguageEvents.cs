using Logitar.Cms.Core.Languages;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class LanguageEvents
{
  public class LanguageCreatedEventHandler : INotificationHandler<Language.CreatedEvent>
  {
    private readonly CmsContext _context;

    public LanguageCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(Language.CreatedEvent @event, CancellationToken cancellationToken)
    {
      LanguageEntity? language = await _context.Languages.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (language == null)
      {
        language = new(@event);

        _context.Languages.Add(language);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class LanguageSetDefaultEventHandler : INotificationHandler<Language.SetDefaultEvent>
  {
    private readonly CmsContext _context;

    public LanguageSetDefaultEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(Language.SetDefaultEvent @event, CancellationToken cancellationToken)
    {
      LanguageEntity language = await _context.Languages
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.AggregateId}' could not be found.");

      language.SetDefault(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class LanguageUpdatedEventHandler : INotificationHandler<Language.UpdatedEvent>
  {
    private readonly CmsContext _context;

    public LanguageUpdatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(Language.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      LanguageEntity language = await _context.Languages
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.AggregateId}' could not be found.");

      language.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
