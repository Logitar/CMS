using System.Globalization;

namespace Logitar.Cms.Core;

[Trait(Traits.Category, Categories.Unit)]
public class StringExtensionTests
{
  [Theory]
  [InlineData("fr-CA")]
  public void When_it_is_a_valid_culture_info_Then_a_culture_info_is_returned(string name)
  {
    CultureInfo culture = name.GetCultureInfo(nameof(name));

    Assert.Equal(name, culture.Name);
  }

  [Fact]
  public void When_it_is_not_a_valid_culture_info_Then_InvalidLocaleException_is_thrown()
  {
    string name = " Test1234567890!_   ";
    string paramName = nameof(name);

    var exception = Assert.Throws<InvalidLocaleException>(() => name.GetCultureInfo(paramName));

    Assert.Equal(paramName, exception.PropertyName);
    Assert.Equal(name, exception.AttemptedValue);
    Assert.IsType<CultureNotFoundException>(exception.InnerException);
  }
}
