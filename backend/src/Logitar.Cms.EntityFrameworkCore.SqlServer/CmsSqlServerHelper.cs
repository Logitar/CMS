using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.EntityFrameworkCore.SqlServer;

namespace Logitar.Cms.EntityFrameworkCore.SqlServer;

internal class CmsSqlServerHelper : SqlServerHelper, ICmsSqlHelper
{
  public IUpdateBuilder Update(TableId table) => new SqlServerUpdateBuilder();
}
