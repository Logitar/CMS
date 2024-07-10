using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Core.Languages;
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
    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueId == id, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<Language?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string codeNormalized = CmsDb.Normalize(locale);

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.CodeNormalized == codeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<Language> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageEntity language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.IsDefault, cancellationToken)
      ?? throw new InvalidOperationException("The default language entity could not be found.");

    return await MapAsync(language, cancellationToken);
  }

  private async Task<Language> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
    => (await MapAsync([language], cancellationToken)).Single();
  private async Task<IReadOnlyCollection<Language>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToArray();
  }
}
