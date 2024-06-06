using Logitar.Cms.Models.Index;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Controllers;

[ApiController]
[Route("api")]
public class IndexController : ControllerBase
{
  [HttpGet]
  public ActionResult<ApiVersion> Get() => Ok(ApiVersion.Current);
}
