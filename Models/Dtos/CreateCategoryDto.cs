using System;
using System.ComponentModel.DataAnnotations;

namespace APIEcommerce.Models.Dtos;

public class CreateCategoryDto
{
  [Required(ErrorMessage = "Name is required")]
  [MaxLength(50, ErrorMessage = "Name must not exceed 50 characters")]
  [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
  public string Name { get; set; } = string.Empty;
}
