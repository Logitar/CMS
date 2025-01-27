using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Logitar.Cms.Infrastructure.PostgreSQL;

public class PostgresCommandHelper : ICommandHelper
{
  public IDeleteBuilder Delete(TableId table) => new PostgresDeleteBuilder(table);
  public IUpdateBuilder Update() => new PostgresUpdateBuilder();
}
