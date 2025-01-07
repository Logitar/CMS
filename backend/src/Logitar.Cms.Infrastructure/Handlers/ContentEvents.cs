using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ContentEvents : INotificationHandler<ContentCreated>,
  INotificationHandler<ContentLocaleChanged>
{
  private readonly ICommandHelper _commandHelper;
  private readonly CmsContext _context;

  public ContentEvents(ICommandHelper commandHelper, CmsContext context)
  {
    _commandHelper = commandHelper;
    _context = context;
  }

  public async Task Handle(ContentCreated @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content == null)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .SingleOrDefaultAsync(x => x.StreamId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'StreamId={@event.ContentTypeId}' could not be found.");

      content = new(contentType, @event);

      _context.Contents.Add(content);

      await _context.SaveChangesAsync(cancellationToken);

      // TODO(fpion): update indices
    }
  }

  public async Task Handle(ContentLocaleChanged @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content != null && content.Version == (@event.Version - 1))
    {
      LanguageEntity? language = null;
      if (@event.LanguageId.HasValue)
      {
        language = await _context.Languages
          .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found.");
      }

      content.SetLocale(language, @event);

      await _context.SaveChangesAsync(cancellationToken);

      // TODO(fpion): update indices

      ContentLocaleEntity locale = content.Locales.Single(l => l.LanguageId == language?.LanguageId);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.ContentLocaleName, locale.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.FieldIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.ContentLocaleName, locale.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.ContentLocaleId, Operators.IsEqualTo(locale.ContentLocaleId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }
}
