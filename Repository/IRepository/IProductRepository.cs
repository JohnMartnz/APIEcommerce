using System;
using APIEcommerce.Models;

namespace APIEcommerce.Repository.IRepository;

public interface IProductRepository
{
  ICollection<Product> GetProducts();
  IReadOnlyList<Product> GetProductsForCategory(int categoryId);
  ICollection<Product> SearchProducts(string searchTerm);
  Product? GetProduct(int productId);
  bool BuyProduct(string productName, int quantity);
  bool ProductExists(int productId);
  bool ProductExists(string productName);
  bool CreateProduct(Product product);
  bool UpdateProduct(Product product);
  bool DeleteProduct(Product product);
  bool Save();
}
