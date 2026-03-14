using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using AutoMapper;

namespace APIEcommerce.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<User, CreateUserDto>().ReverseMap();
    CreateMap<User, UserLoginDto>().ReverseMap();
    CreateMap<User, UserLoginResponseDto>().ReverseMap();
    CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
    CreateMap<ApplicationUser, UserDto>().ReverseMap();
  }
}
