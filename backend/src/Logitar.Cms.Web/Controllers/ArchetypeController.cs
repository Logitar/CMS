using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Archetypes.Commands;
using Logitar.Cms.Core.Archetypes.Queries;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
//[Authorize] // TODO(fpion): Authorization
[Route("api/archetypes")]
public class ArchetypeController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public ArchetypeController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<Archetype>> CreateAsync([FromBody] CreateArchetypePayload payload, CancellationToken cancellationToken = default)
  {
    Archetype archetype = await _pipeline.ExecuteAsync(new CreateArchetypeCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/archetypes/{id}", new Dictionary<string, string>
    {
      ["id"] = archetype.Id.ToString()
    });
    return Created(location, archetype);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Archetype>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Archetype? archetype = await _pipeline.ExecuteAsync(new ReadArchetypeQuery(id, UniqueName: null), cancellationToken);
    return archetype == null ? NotFound() : Ok(archetype);
  }

  [HttpGet("unique-name:{uniqueName}")]
  public async Task<ActionResult<Archetype>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    Archetype? archetype = await _pipeline.ExecuteAsync(new ReadArchetypeQuery(Id: null, uniqueName), cancellationToken);
    return archetype == null ? NotFound() : Ok(archetype);
  }
}
