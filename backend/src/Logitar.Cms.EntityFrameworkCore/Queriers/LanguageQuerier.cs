using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<LanguageEntity> _languages;

  public LanguageQuerier(IActorService actorService, CmsContext context)
  {
    _actorService = actorService;
    _languages = context.Languages;
  }

  public async Task<Language> ReadAsync(LanguageAggregate language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity 'AggregateId={language.Id.AggregateId}' could not be found.");
  }
  public async Task<Language?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<Language?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<Language?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string localeNormalized = locale.Trim().ToUpper();

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.LocaleNormalized == localeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  private async Task<Language> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = language.GetActorIds();
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToLanguage(language);
  }
}
