using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Repositories;

internal class LanguageRepository : EventStore, ILanguageRepository
{
  public LanguageRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task SaveAsync(LanguageAggregate language, CancellationToken cancellationToken)
  {
    await base.SaveAsync(language, cancellationToken);
  }
}
