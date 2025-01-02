using Logitar.Cms.Core;
using Logitar.EventSourcing;

namespace Logitar.Cms;

internal class TestApplicationContext : IApplicationContext
{
  private readonly TestContext _context;

  public TestApplicationContext(TestContext context)
  {
    _context = context;
  }

  public ActorId? ActorId => _context.Actor == null ? null : new(_context.Actor.Id);
}
