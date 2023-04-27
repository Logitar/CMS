using System.Globalization;

namespace Logitar.Cms.Core;

[Trait(Traits.Category, Categories.Unit)]
public class StringExtensionTests
{
  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void When_it_is_a_null_or_white_space_string_Then_it_returns_a_null_uri(string? url)
  {
    Assert.Null(url!.GetUri(nameof(url)));
  }

  [Theory]
  [InlineData("fr-CA")]
  public void When_it_is_a_valid_culture_info_Then_a_culture_info_is_returned(string name)
  {
    CultureInfo culture = name.GetCultureInfo(nameof(name));

    Assert.Equal(name, culture.Name);
  }

  [Theory]
  [InlineData("https://www.test.com/")]
  public void When_it_is_a_valid_url_Then_an_uri_is_returned(string url)
  {
    Uri? uri = url.GetUri(nameof(url));
    Assert.NotNull(uri);

    Assert.Equal(url, uri.ToString());
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

  [Fact]
  public void When_it_is_not_a_valid_url_Then_InvalidUrlException_is_thrown()
  {
    string url = "test";
    string paramName = nameof(url);

    var exception = Assert.Throws<InvalidUrlException>(() => url.GetUri(paramName));

    Assert.Equal(paramName, exception.PropertyName);
    Assert.Equal(url, exception.AttemptedValue);
  }
}
