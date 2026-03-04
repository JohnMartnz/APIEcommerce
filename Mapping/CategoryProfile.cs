using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using AutoMapper;

namespace APIEcommerce.Mapping;

public class CategoryProfile : Profile
{
  public CategoryProfile()
  {
    CreateMap<Category, CategoryDto>().ReverseMap();
    CreateMap<Category, CreateCategoryDto>().ReverseMap();
  }
}
