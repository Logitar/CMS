using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("cms)]
public class AppController : Controller
{
  [HttpGet("{**anything}")]
  public ActionResult Index() => View();
}
