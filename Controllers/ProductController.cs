using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APIEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            IEnumerable<Product> products = _productRepository.GetProducts();
            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("id:int", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Product> GetProduct(int id)
        {
            Product? product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} does not exist");
            }
            ProductDto productDto = _mapper.Map<ProductDto>(product);

            return Ok(productDto);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", $"Product with name {createProductDto.Name} already exists");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"Category with ID {createProductDto.CategoryId} does not exist");
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(createProductDto);
            // Adding Image
            UploadProductImage(createProductDto, product);

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong trying to create {createProductDto.Name}");
                return StatusCode(500, ModelState);
            }

            Product createdProduct = _productRepository.GetProduct(product.ProductId)!;
            ProductDto productDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new { id = product.ProductId }, productDto);
        }

        private void UploadProductImage(dynamic productDto, Product product)
        {
            if (productDto.Image != null)
            {
                string fileName = product.ProductId + Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                string filePath = Path.Combine(imageFolder, fileName);
                FileInfo file = new FileInfo(filePath);

                if (file.Exists)
                {
                    file.Delete();
                }

                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                productDto.Image.CopyTo(fileStream);
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = $"{baseUrl}/ProductsImages/{fileName}";
                product.ImageUrlLocal = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400";
            }
        }

        [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> GetProductForCategory(int categoryId)
        {
            IReadOnlyList<Product> products = _productRepository.GetProductsForCategory(categoryId);

            if (products.Count == 0)
            {
                return NotFound($"Products with category id {categoryId} don't exist");
            }

            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }

        [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> SearchProducts(string searchTerm)
        {
            IEnumerable<Product> products = _productRepository.SearchProducts(searchTerm);

            if (!products.Any())
            {
                return NotFound($"Products with {searchTerm} don't exist");
            }

            List<ProductDto> productsDto = _mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }

        [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Product>> BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest($"Product name is invalid");
            }

            if (quantity <= 0)
            {
                return BadRequest($"Quantity is invalid");
            }

            if (!_productRepository.ProductExists(name))
            {
                return BadRequest($"Product with name {name} does not exists");
            }

            if (!_productRepository.BuyProduct(name, quantity))
            {
                ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {name} o la cantidad es mayor a la solicitada");
                return BadRequest(ModelState);
            }

            string units = quantity == 1 ? "unidad" : "unidades";

            return Ok($"Se compró {quantity} {units} del producto '{name}'");
        }

        [HttpPatch("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(productId))
            {
                ModelState.AddModelError("CustomError", $"Product with ID {productId} does not exist");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"Category with ID {updateProductDto.CategoryId} does not exist");
                return BadRequest(ModelState);
            }

            Product product = _mapper.Map<Product>(updateProductDto);
            product.ProductId = productId;

            UploadProductImage(updateProductDto, product);

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong trying to update {updateProductDto.Name}");
                return StatusCode(500, ModelState);
            }

            Product createdProduct = _productRepository.GetProduct(product.ProductId)!;
            ProductDto productDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProduct", new { id = product.ProductId }, productDto);
        }

        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int productId)
        {
            if (productId == 0)
            {
                return BadRequest(ModelState);
            }

            Product? product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound($"Product with ID {productId} does not exist");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong trying to delete {product.Name}");
            }

            return NoContent();
        }
    }
}
