using Logitar.EventSourcing;
using System.Globalization;

namespace Logitar.Cms.Core.Languages;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageAggregateTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly CultureInfo _locale = CultureInfo.GetCultureInfo("fr-CA");

  [Fact]
  public void It_should_return_the_correct_string_representation()
  {
    LanguageAggregate language = new(_actorId, _locale);
    string s = $"{_locale.DisplayName} ({_locale.Name})";

    Assert.Equal(s, language.ToString().Split('|').First().Trim());
  }

  [Fact]
  public void When_is_default_has_not_changed_Then_same_change_count()
  {
    LanguageAggregate language = new(_actorId, _locale);
    language.SetDefault(_actorId);
    int changeCount = language.Changes.Count;

    language.SetDefault(_actorId);
    Assert.Equal(changeCount, language.Changes.Count);
  }

  [Theory]
  [InlineData("fr-CA")]
  public void When_it_is_constructed_with_id_Then_it_has_correct_id(string id)
  {
    AggregateId aggregateId = new(id);
    LanguageAggregate language = new(aggregateId);

    Assert.Equal(aggregateId, language.Id);
  }

  [Fact]
  public void When_it_is_constructed_with_invalid_arguments_Then_ValidationException_is_thrown()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(()
      => new LanguageAggregate(_actorId, CultureInfo.InvariantCulture));

    Assert.Equal("LocaleValidator", exception.Errors.Single().ErrorCode);
  }

  [Fact]
  public void When_it_is_constructed_with_valid_arguments_Then_it_is_created()
  {
    LanguageAggregate language = new(_actorId, _locale);

    Assert.Equal(_actorId, language.CreatedById);
    Assert.Equal(_actorId, language.UpdatedById);
    Assert.Equal(_locale, language.Locale);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void When_it_is_set_default_Then_is_default_is_set(bool isDefault)
  {
    LanguageAggregate language = new(_actorId, _locale);
    language.SetDefault(_actorId, isDefault);

    Assert.Equal(isDefault, language.IsDefault);
  }
}
