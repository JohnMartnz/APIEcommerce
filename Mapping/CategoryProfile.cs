using System;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using Mapster;

namespace APIEcommerce.Mapping;

public class CategoryProfile
{
  public static void Configure()
  {
    TypeAdapterConfig<Category, CategoryDto>.NewConfig();
    TypeAdapterConfig<CategoryDto, Category>.NewConfig();

    TypeAdapterConfig<IEnumerable<Category>, List<CategoryDto>>.NewConfig();
    TypeAdapterConfig<ICollection<Category>, List<CategoryDto>>.NewConfig();

    TypeAdapterConfig<Category, CreateCategoryDto>.NewConfig();
    TypeAdapterConfig<CreateCategoryDto, Category>.NewConfig();
  }
}
