using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Languages;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core.Languages;
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

  public async Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken)
  {
    string localeNormalized = CmsDb.Helper.Normalize(locale.Code);

    string? aggregateId = await _languages.AsNoTracking()
      .Where(x => x.LocaleNormalized == localeNormalized)
      .Select(x => x.AggregateId)
      .SingleOrDefaultAsync(cancellationToken);

    return aggregateId == null ? null : new LanguageId(aggregateId);
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'AggregateId={language.Id}' could not be found.");
  }
  public async Task<LanguageModel?> ReadAsync(LanguageId id, CancellationToken cancellationToken)
  {
    return await ReadAsync(id.ToGuid(), cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }
  public async Task<LanguageModel?> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    string localeNormalized = CmsDb.Helper.Normalize(locale);

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.LocaleNormalized == localeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageEntity language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.IsDefault, cancellationToken)
      ?? throw new InvalidOperationException("The default language entity could not be found.");

    return await MapAsync(language, cancellationToken);
  }

  public async Task<SearchResults<LanguageModel>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(CmsDb.Languages.Table).SelectAll(CmsDb.Languages.Table)
      .ApplyIdFilter(payload, CmsDb.Languages.Id);
    _searchHelper.ApplyTextSearch(builder, payload.Search, CmsDb.Languages.Locale, CmsDb.Languages.DisplayName, CmsDb.Languages.EnglishName, CmsDb.Languages.NativeName);

    IQueryable<LanguageEntity> query = _languages.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LanguageEntity>? ordered = null;
    foreach (LanguageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LanguageSort.Code:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Locale) : query.OrderBy(x => x.Locale))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Locale) : ordered.ThenBy(x => x.Locale));
          break;
        case LanguageSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LanguageSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case LanguageSort.EnglishName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EnglishName) : query.OrderBy(x => x.EnglishName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EnglishName) : ordered.ThenBy(x => x.EnglishName));
          break;
        case LanguageSort.NativeName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.NativeName) : query.OrderBy(x => x.NativeName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.NativeName) : ordered.ThenBy(x => x.NativeName));
          break;
        case LanguageSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;
    query = query.ApplyPaging(payload);

    LanguageEntity[] languages = await query.ToArrayAsync(cancellationToken);
    IEnumerable<LanguageModel> items = await MapAsync(languages, cancellationToken);

    return new SearchResults<LanguageModel>(items, total);
  }

  private async Task<LanguageModel> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return languages.Select(mapper.ToLanguage).ToArray().AsReadOnly();
  }
}
