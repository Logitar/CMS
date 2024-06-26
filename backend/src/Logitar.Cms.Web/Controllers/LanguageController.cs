﻿using Logitar.Cms.Contracts.Localization;
using Logitar.Cms.Contracts.Search;
using Logitar.Cms.Core;
using Logitar.Cms.Core.Localization.Commands;
using Logitar.Cms.Core.Localization.Queries;
using Logitar.Cms.Web.Extensions;
using Logitar.Cms.Web.Models.Languages;
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
  public async Task<ActionResult<Language>> CreateAsync([FromBody] CreateLanguagePayload payload, CancellationToken cancellationToken)
  {
    Language language = await _pipeline.ExecuteAsync(new CreateLanguageCommand(payload), cancellationToken);
    Uri location = HttpContext.BuildLocation("api/languages", new Dictionary<string, string>
    {
      ["id"] = language.Id.ToString()
    });
    return Created(location, language);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Language>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(id, Code: null, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("code:{code}")]
  public async Task<ActionResult<Language>> ReadAsync(string code, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, code, IsDefault: false), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet("default")]
  public async Task<ActionResult<Language>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new ReadLanguageQuery(Id: null, Code: null, IsDefault: true), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Language>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchResults<Language> languages = await _pipeline.ExecuteAsync(new SearchLanguagesQuery(parameters.ToPayload()), cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}/default")]
  public async Task<ActionResult<Language>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await _pipeline.ExecuteAsync(new SetDefaultLanguageCommand(id), cancellationToken);
    return language == null ? NotFound() : Ok(language);
  }
}
