using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL;

internal class PostgresSearchHelper : SearchHelper
{
  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}
