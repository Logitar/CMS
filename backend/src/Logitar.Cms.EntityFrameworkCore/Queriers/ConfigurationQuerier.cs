using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.EntityFrameworkCore.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Queriers;

internal class ConfigurationQuerier : IConfigurationQuerier
{
  private readonly IActorService _actorService;

  public ConfigurationQuerier(IActorService actorService)
  {
    _actorService = actorService;
  }

  public async Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = [configuration.CreatedBy, configuration.UpdatedBy];
    IReadOnlyCollection<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return mapper.ToConfiguration(configuration);
  }
}
