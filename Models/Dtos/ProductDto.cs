using System;
using System.ComponentModel.DataAnnotations;

namespace APIEcommerce.Models.Dtos;

public class ProductDto
{
  public int ProductId { get; set; }

  public required string Name { get; set; }
  public string Description { get; set; } = string.Empty;

  public decimal Price { get; set; }

  public string ImageUrl { get; set; } = string.Empty;

  public string SKU { get; set; } = string.Empty; // PRO-001-BLK-M

  public int Stock { get; set; }

  public DateTime CreationDate { get; set; } = DateTime.Now;

  public DateTime? UpdateDate { get; set; } = null;

  // public required CategoryDto Category { get; set; }
  public int CategoryId { get; set; }

  public string CategoryName { get; set; } = string.Empty;
}
