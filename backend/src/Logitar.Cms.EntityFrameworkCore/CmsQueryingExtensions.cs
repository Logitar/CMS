using Logitar.Cms.Contracts.Search;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class CmsQueryingExtensions
{
  public static IQueryBuilder ApplyIdFilter(this IQueryBuilder query, SearchPayload payload, ColumnId column)
  {
    if (payload.Ids.Count > 0)
    {
      string[] ids = payload.Ids.Select(id => id.ToString()).Distinct().ToArray();
      query.Where(column, Operators.IsIn(ids));
    }

    return query;
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    return query.ApplyPaging(payload.Skip, payload.Limit);
  }
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int limit)
  {
    if (skip > 0)
    {
      query = query.Skip(skip);
    }
    if (limit > 0)
    {
      query = query.Take(limit);
    }

    return query;
  }
}
