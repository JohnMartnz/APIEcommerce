using System;
using APIEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace APIEcommerce.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {

  }

  public DbSet<Category> Categories { get; set; }

  public DbSet<Product> Products { get; set; }
}
