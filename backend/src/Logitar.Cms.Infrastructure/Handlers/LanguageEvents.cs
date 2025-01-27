using Logitar.Cms.Core.Localization.Events;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class LanguageEvents : INotificationHandler<LanguageCreated>,
  INotificationHandler<LanguageLocaleChanged>,
  INotificationHandler<LanguageSetDefault>
{
  private readonly ICommandHelper _commandHelper;
  private readonly CmsContext _context;

  public LanguageEvents(ICommandHelper commandHelper, CmsContext context)
  {
    _commandHelper = commandHelper;
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

  public async Task Handle(LanguageLocaleChanged @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language != null && language.Version == (@event.Version - 1))
    {
      language.SetLocale(@event);

      await _context.SaveChangesAsync(cancellationToken);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.LanguageCode, language.CodeNormalized))
        .Where(new OperatorCondition(CmsDb.FieldIndex.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.LanguageCode, language.CodeNormalized))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.PublishedContents.LanguageCode, language.CodeNormalized))
        .Where(new OperatorCondition(CmsDb.PublishedContents.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }

  public async Task Handle(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language != null && language.Version == (@event.Version - 1))
    {
      language.SetDefault(@event);

      await _context.SaveChangesAsync(cancellationToken);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.LanguageIsDefault, language.IsDefault))
        .Where(new OperatorCondition(CmsDb.FieldIndex.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.LanguageIsDefault, language.IsDefault))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.PublishedContents.LanguageIsDefault, language.IsDefault))
        .Where(new OperatorCondition(CmsDb.PublishedContents.LanguageId, Operators.IsEqualTo(language.LanguageId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }
}
