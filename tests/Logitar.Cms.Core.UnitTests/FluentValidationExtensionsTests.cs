using Logitar.Cms.Core.Configurations;
using System.Globalization;

namespace Logitar.Cms.Core;

[Trait(Traits.Category, Categories.Unit)]
public class FluentValidationExtensionsTests
{
  private readonly ReadOnlyUsernameSettings _usernameSettings = new();

  [Fact]
  public void When_locale_has_invalid_lcid_Then_it_is_not_valid()
  {
    CultureInfo locale = CultureInfo.GetCultures(CultureTypes.AllCultures).First(c => c.LCID == 4096);
    Assert.False(FluentValidationExtensions.BeAValidLocale(locale));
  }

  [Fact]
  public void When_locale_has_invalid_name_Then_it_is_not_valid()
  {
    Assert.False(FluentValidationExtensions.BeAValidLocale(CultureInfo.InvariantCulture));
  }

  [Fact]
  public void When_locale_has_valid_lcid_and_name_Then_it_is_valid()
  {
    Assert.True(FluentValidationExtensions.BeAValidLocale(CultureInfo.GetCultureInfo("fr-CA")));
  }

  [Fact]
  public void When_locale_is_null_Then_it_is_valid()
  {
    Assert.True(FluentValidationExtensions.BeAValidLocale(null));
  }

  [Fact]
  public void When_string_is_empty_Then_it_is_not_null_or_not_empty()
  {
    Assert.False(FluentValidationExtensions.BeNullOrNotEmpty(string.Empty));
  }

  [Fact]
  public void When_string_is_white_space_Then_it_is_not_null_or_not_empty()
  {
    Assert.False(FluentValidationExtensions.BeNullOrNotEmpty("   "));
  }

  [Fact]
  public void When_string_is_not_empty_Then_it_is_null_or_not_empty()
  {
    Assert.True(FluentValidationExtensions.BeNullOrNotEmpty(" Hello World! "));
  }

  [Fact]
  public void When_string_is_null_Then_it_is_null_or_not_empty()
  {
    Assert.True(FluentValidationExtensions.BeNullOrNotEmpty(null));
  }

  [Fact]
  public void When_username_allowed_characters_are_null_Then_any_username_is_valid()
  {
    ReadOnlyUsernameSettings usernameSettings = new() { AllowedCharacters = null };
    Assert.True(FluentValidationExtensions.BeAValidUsername("!/$%?&*()", usernameSettings));
  }

  [Fact]
  public void When_username_contains_a_character_not_allowed_Then_it_is_not_valid()
  {
    Assert.False(FluentValidationExtensions.BeAValidUsername("!/$%?&*()", _usernameSettings));
  }

  [Fact]
  public void When_username_is_null_Then_it_is_valid()
  {
    Assert.True(FluentValidationExtensions.BeAValidUsername(null, _usernameSettings));
  }

  [Fact]
  public void When_username_only_contains_allowed_characters_Then_it_is_valid()
  {
    Assert.True(FluentValidationExtensions.BeAValidUsername("test-user+1@my_domain.com", _usernameSettings));
  }
}
