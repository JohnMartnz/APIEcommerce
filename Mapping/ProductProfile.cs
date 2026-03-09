using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using AutoMapper;

namespace APIEcommerce.Mapping;

public class ProductProfile : Profile
{
  public ProductProfile()
  {
    CreateMap<Product, ProductDto>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
    .ReverseMap();
    CreateMap<Product, CreateProductDto>().ReverseMap();
    CreateMap<Product, UpdateProductDto>().ReverseMap();
  }
}
