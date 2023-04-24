namespace Logitar.Cms.Contracts.Resources;

public interface IResourceService
{
  IEnumerable<Locale> GetLocales();
}
