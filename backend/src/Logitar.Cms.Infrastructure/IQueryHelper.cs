using Logitar.Cms.Core.Search;
using Logitar.Data;

namespace Logitar.Cms.Infrastructure;

public interface IQueryHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder query, TextSearch search, params ColumnId[] columns);

  IQueryBuilder From(TableId table);
}
