using Logitar.Cms.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Cms.Web.Models.Search;

public record SearchParameters
{
  protected const string DescendingKeyword = "DESC";
  protected const char SortSeparator = '.';

  [FromQuery(Name = "ids")]
  public List<Guid>? IdIn { get; set; }

  [FromQuery(Name = "search_terms")]
  public List<string>? SearchTerms { get; set; }

  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string>? Sort { get; set; }

  [FromQuery(Name = "skip")]
  public int? Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int? Limit { get; set; }

  protected void FillPayload(SearchPayload payload)
  {
    payload.IdIn = IdIn;

    if (SearchTerms != null)
    {
      payload.Search = new TextSearch(SearchTerms.Select(value => new SearchTerm(value)), SearchOperator);
    }

    if (Sort != null)
    {
      payload.Sort = new List<SortOption>(capacity: Sort.Count);
      foreach (string sort in Sort)
      {
        SortOption? option = ParseSortOption(sort);
        if (option != null)
        {
          payload.Sort.Add(option);
        }
      }
    }

    payload.Skip = Skip;
    payload.Limit = Limit;
  }

  protected SortOption ParseSortOption(string sort)
  {
    int index = sort.IndexOf(SortSeparator);
    if (index < 0)
    {
      return new SortOption(sort);
    }

    string field = sort[(index + 1)..];
    bool isDescending = sort[..index].Equals(DescendingKeyword, StringComparison.InvariantCultureIgnoreCase);
    return new SortOption(field, isDescending);
  }
}
