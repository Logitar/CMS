using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Languages;

internal class LanguageCreatedEventHandler : INotificationHandler<LanguageCreatedEvent>
{
  private readonly CmsContext _context;

  public LanguageCreatedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageCreatedEvent @event, CancellationToken cancellationToken)
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
