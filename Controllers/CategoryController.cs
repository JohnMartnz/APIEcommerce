using APIEcommerce.Constants;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace APIEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoryController : ControllerBase
    {
        private const string ERROR_KEY = "CustomError";
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            ICollection<Category> categories = _categoryRepository.GetCategories();
            List<CategoryDto> categoriesDto = new List<CategoryDto>();
            foreach (Category category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }

            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CategoryDto> GetCategory(int id)
        {
            Category? category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} does not exist");
            }

            CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError(ERROR_KEY, "Category already exists");
                return BadRequest(ModelState);
            }

            Category category = _mapper.Map<Category>(createCategoryDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError(ERROR_KEY, $"Something went wrong trying to save {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with ID {id} does not exist");
            }

            if (updateCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError(ERROR_KEY, "Category already exists");
                return BadRequest(ModelState);
            }

            Category category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError(ERROR_KEY, $"Something went wrong trying to update {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with ID {id} does not exist");
            }

            Category? category = _categoryRepository.GetCategory(id);
            if (category == null)
            {
                ModelState.AddModelError(ERROR_KEY, $"Category with ID {id} does not exist");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError(ERROR_KEY, $"Something went wrong trying to update {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
