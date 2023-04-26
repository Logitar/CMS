using Logitar.Cms.Contracts.Resources;
using System.Globalization;

namespace Logitar.Cms.Core.Resources;

[Trait(Traits.Category, Categories.Unit)]
public class ResourceServiceTests
{
  private readonly ResourceService _service = new();

  [Fact]
  public void When_locales_are_fetched_Then_all_locales_are_valid()
  {
    CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
    var lcid4096 = cultures.Where(c => c.LCID == 4096).ToArray();

    IEnumerable<Locale> locales = _service.GetLocales();
    foreach (Locale locale in locales)
    {
      CultureInfo culture = CultureInfo.GetCultureInfo(locale.Code);

      Assert.False(string.IsNullOrWhiteSpace(culture.Name));
      Assert.NotEqual(4096, culture.LCID);
      Assert.Equal(culture.DisplayName, locale.DisplayName);
    }
  }

  [Fact]
  public void When_locales_are_fetched_Then_locales_are_ordered_by_display_name()
  {
    IEnumerable<Locale> locales = _service.GetLocales();
    string actual = string.Join(';', locales.Select(l => l.Code));
    string expected = string.Join(';', locales.OrderBy(l => l.DisplayName).Select(l => l.Code));

    Assert.Equal(expected, actual);
  }
}
