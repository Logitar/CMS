using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Localization.Commands;
using Logitar.Cms.Core.Localization.Queries;
using Logitar.Cms.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  private readonly IRequestPipeline _pipeline;

  public LanguageController(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  [HttpPost]
  public async Task<ActionResult<Language>> CreateAsync([FromBody] CreateLanguagePayload payload, CancellationToken cancellationToken = default)
  {
    Language language = await _pipeline.ExecuteAsync(new CreateLanguageCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/languages/{id}", new Dictionary<string, string>
    {
      ["id"] = language.Id.ToString()
    });
    return Created(location, language);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Language>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id, Locale: null), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("locale:{locale}")]
  public async Task<ActionResult<Language>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, locale), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }
}
