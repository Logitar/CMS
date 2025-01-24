using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Logitar.Cms.Infrastructure.SqlServer;

public class SqlServerCommandHelper : ICommandHelper
{
  public IDeleteBuilder Delete(TableId table) => new SqlServerDeleteBuilder(table);
  public IUpdateBuilder Update() => new SqlServerUpdateBuilder();
}
