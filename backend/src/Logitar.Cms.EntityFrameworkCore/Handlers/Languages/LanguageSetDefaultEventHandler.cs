using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Languages;

internal class LanguageSetDefaultEventHandler : INotificationHandler<LanguageSetDefaultEvent>
{
  private readonly CmsContext _context;

  public LanguageSetDefaultEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageSetDefaultEvent @event, CancellationToken cancellationToken)
  {
    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.AggregateId}' could not be found.");

    language.SetDefault(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
