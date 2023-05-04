using Logitar.Cms.Contracts.Resources;
using System.Globalization;

namespace Logitar.Cms.Core.Resources;

internal class ResourceService : IResourceService
{
  private static readonly IEnumerable<Locale> _locales = CultureInfo.GetCultures(CultureTypes.AllCultures)
    .Select(Locale.From).OrderBy(x => x.NativeName);
    .Where(culture => !string.IsNullOrWhiteSpace(culture.Name) && culture.LCID != 4096)
    .Select(Locale.From)
    .OrderBy(x => x.NativeName);

  public IEnumerable<Locale> GetLocales() => _locales;
}
