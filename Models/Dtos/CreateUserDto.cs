using System;
using System.ComponentModel.DataAnnotations;

namespace APIEcommerce.Models.Dtos;

public class CreateUserDto
{
  [Required(ErrorMessage = "Name field is required")]
  public string? Name { get; set; }

  [Required(ErrorMessage = "Username field is required")]
  public string? Username { get; set; }

  [Required(ErrorMessage = "Password field is required")]
  public string? Password { get; set; }

  [Required(ErrorMessage = "Role field is required")]
  public string? Role { get; set; }
}
