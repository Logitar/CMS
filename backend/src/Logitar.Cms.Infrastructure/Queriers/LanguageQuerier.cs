using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Queriers;

internal class LanguageQuerier : ILanguageQuerier
{
  private readonly DbSet<LanguageEntity> _languages;

  public LanguageQuerier(CmsContext context)
  {
    _languages = context.Languages;
  }

  public async Task<LanguageId> FindDefaultIdAsync(CancellationToken cancellationToken)
  {
    string streamId = await _languages.AsNoTracking()
      .Where(x => x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException("The default language entity could not be found.");

    return new LanguageId(streamId);
  }
  public async Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken)
  {
    string codeNormalized = CmsDb.Helper.Normalize(locale.Value);

    string? streamId = await _languages.AsNoTracking()
      .Where(x => x.CodeNormalized == codeNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new LanguageId(streamId);
  }

  public async Task<LanguageModel> ReadAsync(Language language, CancellationToken cancellationToken)
  {
    return await ReadAsync(language.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The language 'StreamId={language.Id}' could not be found.");
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
    string codeNormalized = CmsDb.Helper.Normalize(locale);

    LanguageEntity? language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.CodeNormalized == codeNormalized, cancellationToken);

    return language == null ? null : await MapAsync(language, cancellationToken);
  }

  public async Task<LanguageModel> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    LanguageEntity language = await _languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.IsDefault)
      ?? throw new InvalidOperationException("The default language entity could not be found.");

    return await MapAsync(language, cancellationToken);
  }

  private async Task<LanguageModel> MapAsync(LanguageEntity language, CancellationToken cancellationToken)
  {
    return (await MapAsync([language], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LanguageModel>> MapAsync(IEnumerable<LanguageEntity> languages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = languages.SelectMany(language => language.GetActorIds());
    await Task.Delay(1, cancellationToken); // TODO(fpion): actors
    Mapper mapper = new(); // TODO(fpion): actors

    return languages.Select(mapper.ToLanguage).ToArray();
  }
}
