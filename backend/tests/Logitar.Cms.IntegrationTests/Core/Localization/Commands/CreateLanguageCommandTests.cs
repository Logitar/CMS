using FluentValidation.Results;
using Logitar.Cms.Contracts.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateLanguageCommandTests : IntegrationTests
{
  public CreateLanguageCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new language.")]
  public async Task It_should_create_a_new_language()
  {
    CreateLanguagePayload payload = new("fr-CA");
    CreateLanguageCommand command = new(payload);
    Language language = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, language.Id);
    Assert.Equal(1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(language.CreatedOn, language.UpdatedOn);

    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale.Trim(), language.Locale.Code);

    Assert.NotNull(await CmsContext.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(language.Id).Value));
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when the locale is already used.")]
  public async Task It_should_throw_LocaleAlreadyUsedException_when_the_locale_is_already_used()
  {
    CreateLanguagePayload payload = new(Faker.Locale);
    CreateLanguageCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.Locale, exception.Locale);
    Assert.Equal(nameof(payload.Locale), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateLanguagePayload payload = new("test");
    CreateLanguageCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(LocaleValidator), error.ErrorCode);
    Assert.Equal(nameof(payload.Locale), error.PropertyName);
  }
}
