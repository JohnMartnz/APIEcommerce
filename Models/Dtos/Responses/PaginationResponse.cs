using System;

namespace APIEcommerce.Models.Dtos.Responses;

public class PaginationResponse<T> where T : class
{
  public ICollection<T> Items { get; set; } = new List<T>();
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalPages { get; set; }
  public int TotalItems { get; set; }
}
