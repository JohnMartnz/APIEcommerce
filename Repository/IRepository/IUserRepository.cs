using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;

namespace APIEcommerce.Repository.IRepository;

public interface IUserRepository
{
  ICollection<User> GetUsers();
  User? GetUser(int id);
  bool IsUniqueUser(string username);
  Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
  Task<User> Register(CreateUserDto createUserDto);
}
