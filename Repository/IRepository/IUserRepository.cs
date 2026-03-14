using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;

namespace APIEcommerce.Repository.IRepository;

public interface IUserRepository
{
  ICollection<ApplicationUser> GetUsers();
  ApplicationUser? GetUser(string id);
  bool IsUniqueUser(string username);
  Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
  Task<UserDataDto> Register(CreateUserDto createUserDto);
}
