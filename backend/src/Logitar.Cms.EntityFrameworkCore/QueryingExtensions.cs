using Logitar.Cms.Contracts.Search;
using Logitar.Data;

namespace Logitar.Cms.EntityFrameworkCore;

internal static class QueryingExtensions
{
  public static IQueryBuilder ApplyIdInFilter(this IQueryBuilder builder, ColumnId column, SearchPayload payload)
  {
    if (payload.IdIn == null || payload.IdIn.Count < 1)
    {
      return builder;
    }

    Guid[] uniqueIds = payload.IdIn.Distinct().ToArray();

    return builder.Where(column, Operators.IsIn(uniqueIds));
  }

  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, SearchPayload payload)
  {
    if (payload.Skip.HasValue)
    {
      query = query.Skip(payload.Skip.Value);
    }

    if (payload.Limit.HasValue)
    {
      query = query.Take(payload.Limit.Value);
    }

    return query;
  }
}
