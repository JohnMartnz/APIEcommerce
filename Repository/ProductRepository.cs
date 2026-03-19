using System;
using APIEcommerce.Data;
using APIEcommerce.Models;
using APIEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace APIEcommerce.Repository;

public class ProductRepository : IProductRepository
{
  private readonly ApplicationDbContext _db;

  public ProductRepository(ApplicationDbContext db)
  {
    _db = db;
  }

  public bool BuyProduct(string productName, int quantity)
  {
    if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
    {
      return false;
    }

    Product? product = _db.Products.FirstOrDefault(product => product.Name.ToLower().Trim() == productName.ToLower().Trim());
    if (product == null || product.Stock < quantity)
    {
      return false;
    }

    product.Stock -= quantity;
    _db.Products.Update(product);
    return Save();
  }

  public bool CreateProduct(Product product)
  {
    if (product == null)
    {
      return false;
    }

    product.CreationDate = DateTime.Now;
    product.CreationDate = DateTime.Now;
    _db.Products.Add(product);
    return Save();
  }

  public bool DeleteProduct(Product product)
  {
    if (product == null)
    {
      return false;
    }

    _db.Products.Remove(product);
    return Save();
  }

  public Product? GetProduct(int productId)
  {
    if (productId <= 0)
    {
      return null;
    }

    return _db.Products.Include(product => product.Category).FirstOrDefault(product => product.ProductId == productId);
  }

  public ICollection<Product> GetProducts()
  {
    return _db.Products.Include(product => product.Category).OrderBy(product => product.Name).ToList();
  }

  public IReadOnlyList<Product> GetProductsForCategory(int categoryId)
  {
    if (categoryId <= 0)
    {
      return new List<Product>();
    }

    return _db.Products.Include(product => product.Category).Where(product => product.CategoryId == categoryId).OrderBy(product => product.Name).ToList();
  }

  public bool ProductExists(int productId)
  {
    if (productId <= 0)
    {
      return false;
    }

    return _db.Products.Any(product => product.ProductId == productId);
  }

  public bool ProductExists(string productName)
  {
    if (string.IsNullOrWhiteSpace(productName))
    {
      return false;
    }

    return _db.Products.Any(product => product.Name.ToLower().Trim() == productName.ToLower().Trim());
  }

  public bool Save()
  {
    return _db.SaveChanges() >= 0;
  }

  public ICollection<Product> SearchProducts(string searchTerm)
  {
    string searchTermLower = searchTerm.ToLower().Trim();
    IQueryable<Product> query = _db.Products;
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
      query = query.Where(
        product => product.Name.ToLower().Trim().Contains(searchTermLower) ||
        product.Description.ToLower().Trim().Contains(searchTermLower)
        );
    }

    return query.Include(product => product.Category).OrderBy(product => product.Name).ToList();
  }

  public bool UpdateProduct(Product product)
  {
    if (product == null)
    {
      return false;
    }

    product.UpdateDate = DateTime.Now;
    _db.Products.Update(product);
    return Save();
  }

  public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize)
  {
    return _db.Products.OrderBy(product => product.ProductId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
  }

  public int GetTotalProducts()
  {
    return _db.Products.Count();
  }
}
