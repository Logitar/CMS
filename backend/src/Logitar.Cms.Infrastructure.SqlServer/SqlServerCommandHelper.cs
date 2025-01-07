using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Logitar.Cms.Infrastructure.SqlServer;

public class SqlServerCommandHelper : ICommandHelper
{
  public IUpdateBuilder Update() => new SqlServerUpdateBuilder();
}
