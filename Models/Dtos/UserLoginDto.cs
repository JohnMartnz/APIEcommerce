using System;
using System.ComponentModel.DataAnnotations;

namespace APIEcommerce.Models.Dtos;

public class UserLoginDto
{
  [Required(ErrorMessage = "Username field is required")]
  public string? Username { get; set; }

  [Required(ErrorMessage = "Password field is required")]
  public string? Password { get; set; }
}
