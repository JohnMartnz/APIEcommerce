using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using Mapster;

namespace APIEcommerce.Mapping;

public class UserProfile
{
  public static void Configure()
  {
    TypeAdapterConfig<User, UserDto>.NewConfig();
    TypeAdapterConfig<UserDto, User>.NewConfig();

    TypeAdapterConfig<IEnumerable<ApplicationUser>, List<UserDto>>.NewConfig();

    TypeAdapterConfig<User, CreateUserDto>.NewConfig();
    TypeAdapterConfig<CreateUserDto, User>.NewConfig();
    TypeAdapterConfig<User, UserLoginDto>.NewConfig();
    TypeAdapterConfig<UserLoginDto, User>.NewConfig();
    TypeAdapterConfig<User, UserLoginResponseDto>.NewConfig();
    TypeAdapterConfig<UserLoginResponseDto, User>.NewConfig();
    TypeAdapterConfig<ApplicationUser, UserDataDto>.NewConfig();
    TypeAdapterConfig<UserDataDto, ApplicationUser>.NewConfig();
    TypeAdapterConfig<ApplicationUser, UserDto>.NewConfig();
    TypeAdapterConfig<UserDto, ApplicationUser>.NewConfig();
  }
}
