using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.Localization;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<LanguageEntity> _languages;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public LanguageQuerier(IActorService actorService, CmsContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _languages = context.Languages;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<Language> ReadAsync(LanguageAggregate language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity (AggregateId={language.Id.AggregateId}) could not be found.");
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
    string localeNormalized = CmsDb.Normalize(locale);

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.LocaleNormalized == localeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<Language> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageEntity language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.IsDefault, cancellationToken)
      ?? throw new InvalidOperationException("The default language entity could not be found.");

    return await MapAsync(language, cancellationToken);
  }

  public async Task<SearchResults<Language>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.Languages.Table).SelectAll(CmsDb.Languages.Table)
      .ApplyIdInFilter(CmsDb.Languages.AggregateId, payload);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.Languages.Locale);

    IQueryable<LanguageEntity> query = _languages.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LanguageEntity>? ordered = null;
    if (payload.Sort != null)
    {
      foreach (LanguageSortOption sort in payload.Sort)
      {
        switch (sort.Field)
        {
          case LanguageSort.Locale:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.Locale) : query.OrderBy(x => x.Locale))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.Locale) : ordered.ThenBy(x => x.Locale));
            break;
          case LanguageSort.UpdatedOn:
            ordered = (ordered == null)
              ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
              : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
            break;
        }
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    LanguageEntity[] languages = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Language> items = await MapAsync(languages, cancellationToken);

    return new SearchResults<Language>(items, total);
  }

  private async Task<Language> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<Language>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToArray();
  }
}
