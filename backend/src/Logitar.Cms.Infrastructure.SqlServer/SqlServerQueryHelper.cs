using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Logitar.Cms.Infrastructure.SqlServer;

public class SqlServerQueryHelper : QueryHelper
{
  public override IQueryBuilder From(TableId table) => SqlServerQueryBuilder.From(table);
}
