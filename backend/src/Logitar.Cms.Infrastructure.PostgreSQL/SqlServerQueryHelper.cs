using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Logitar.Cms.Infrastructure.PostgreSQL;

public class PostgreSQLQueryHelper : QueryHelper
{
  public override IQueryBuilder From(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
