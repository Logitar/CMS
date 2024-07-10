using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Languages;

internal class LanguageDeletedEventHandler : INotificationHandler<LanguageDeletedEvent>
{
  private readonly CmsContext _context;

  public LanguageDeletedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageDeletedEvent @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (language != null)
    {
      _context.Languages.Remove(language);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
