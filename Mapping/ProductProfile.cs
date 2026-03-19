using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using Mapster;

namespace APIEcommerce.Mapping;

public class ProductProfile
{
  public static void Configure()
  {
    TypeAdapterConfig<Product, ProductDto>.NewConfig()
      .Map(dest => dest.CategoryName, src => src.Category == null ? null : src.Category.Name);
    TypeAdapterConfig<ProductDto, Product>.NewConfig();

    TypeAdapterConfig<IEnumerable<Product>, List<ProductDto>>.NewConfig();

    TypeAdapterConfig<Product, CreateProductDto>.NewConfig();
    TypeAdapterConfig<CreateProductDto, Product>.NewConfig();
    TypeAdapterConfig<Product, UpdateProductDto>.NewConfig();
    TypeAdapterConfig<UpdateProductDto, Product>.NewConfig();
  }
}
