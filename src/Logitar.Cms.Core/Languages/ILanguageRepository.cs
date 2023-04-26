namespace Logitar.Cms.Core.Languages;

public interface ILanguageRepository
{
  Task SaveAsync(LanguageAggregate language, CancellationToken cancellationToken = default);
}
