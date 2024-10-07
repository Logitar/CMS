using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Languages;
using Moq;

namespace Logitar.Cms.Core.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly ReadLanguageQueryHandler _handler;

  public ReadLanguageQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Theory(DisplayName = "It should return null when no language is found.")]
  [InlineData(null, null)]
  [InlineData("150c358b-f037-46c0-adbb-3ef9967813db", "en")]
  public async Task It_should_return_null_when_no_language_is_found(string? id, string? locale)
  {
    ReadLanguageQuery query = new(id == null ? null : Guid.Parse(id), locale, IsDefault: false);
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many languages are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_languages_are_found()
  {
    Language english = new(new Locale("en"))
    {
      Id = Guid.NewGuid(),
      IsDefault = true
    };
    _languageQuerier.Setup(x => x.ReadAsync(english.Id, _cancellationToken)).ReturnsAsync(english);
    _languageQuerier.Setup(x => x.ReadDefaultAsync(_cancellationToken)).ReturnsAsync(english);

    Language french = new(new Locale("fr"))
    {
      Id = Guid.NewGuid()
    };
    _languageQuerier.Setup(x => x.ReadAsync(french.Locale.Code, _cancellationToken)).ReturnsAsync(french);

    ReadLanguageQuery query = new(english.Id, french.Locale.Code, IsDefault: true);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Language>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
