using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ICollection<UserDto>> GetUsers()
        {
            IEnumerable<User> users = _userRepository.GetUsers();
            List<UserDto> userDto = _mapper.Map<List<UserDto>>(users);

            return Ok(userDto);
        }

        [HttpGet("{id}:int", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<UserDto> GetUser(int id)
        {
            User? user = _userRepository.GetUser(id);
            if (user == null)
            {
                return NotFound($"User with id {id} does not found");
            }

            UserDto userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(createUserDto.Username))
            {
                return BadRequest("Username is required");
            }

            if (!_userRepository.IsUniqueUser(createUserDto.Username))
            {
                return BadRequest("User already exists");
            }

            User result = await _userRepository.Register(createUserDto);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Something went wrong trying to create user {createUserDto.Username}");
            }

            return CreatedAtRoute("GetUser", new { id = result.Id }, result);
        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserLoginResponseDto user = await _userRepository.Login(userLoginDto);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }
    }
}
