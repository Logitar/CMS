using FluentValidation.Results;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core.Shared;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Archetypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateArchetypeCommandTests : IntegrationTests
{
  private readonly IArchetypeRepository _archetypeRepository;

  public CreateArchetypeCommandTests() : base()
  {
    _archetypeRepository = ServiceProvider.GetRequiredService<IArchetypeRepository>();
  }

  [Fact(DisplayName = "It should create a new archetype.")]
  public async Task It_should_create_a_new_archetype()
  {
    CreateArchetypePayload payload = new("Product")
    {
      DisplayName = "  Products  ",
      Description = "    "
    };
    CreateArchetypeCommand command = new(payload);
    Archetype archetype = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, archetype.Id);
    Assert.Equal(2, archetype.Version);
    Assert.Equal(Actor.System, archetype.CreatedBy);
    Assert.Equal(Actor.System, archetype.UpdatedBy);
    Assert.True(archetype.CreatedOn < archetype.UpdatedOn);

    Assert.True(archetype.IsInvariant);
    Assert.Equal(payload.UniqueName.Trim(), archetype.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), archetype.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), archetype.Description);

    Assert.NotNull(await CmsContext.Archetypes.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(archetype.Id).Value));
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ArchetypeAggregate archetype = new(new IdentifierUnit("Product"));
    await _archetypeRepository.SaveAsync(archetype);

    CreateArchetypePayload payload = new(archetype.UniqueName.Value);
    CreateArchetypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ArchetypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateArchetypePayload payload = new("0_Products");
    CreateArchetypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(Identity.Domain.Shared.IdentifierValidator), error.ErrorCode);
    Assert.Equal(nameof(payload.UniqueName), error.PropertyName);
  }
}
