using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIEcommerce.Data;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace APIEcommerce.Repository;

public class UserRepository : IUserRepository
{
  private readonly ApplicationDbContext _db;
  private readonly string? secretKey;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IMapper _mapper;

  public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
  {
    _db = db;
    secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    _userManager = userManager;
    _roleManager = roleManager;
    _mapper = mapper;
  }

  public ICollection<ApplicationUser> GetUsers()
  {
    return _db.ApplicationUsers.OrderBy(user => user.UserName).ToList();
  }

  public ApplicationUser? GetUser(string id)
  {
    return _db.ApplicationUsers.FirstOrDefault(user => user.Id == id);
  }

  public bool IsUniqueUser(string username)
  {
    return !_db.Users.Any(user => user.Username.ToLower().Trim() == username.ToLower().Trim());
  }

  public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
  {
    if (string.IsNullOrEmpty(userLoginDto.Username))
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Username is required"
      };
    }

    ApplicationUser? user = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.UserName != null && user.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
    if (user == null)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Username not found"
      };
    }

    if (userLoginDto.Password == null)
    {
      return new UserLoginResponseDto
      {
        Token = null,
        User = null,
        Message = "Password is required"
      };
    }

    bool isPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);

    if (!isPasswordValid)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Credentials are incorrect"
      };
    }

    // JWT
    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    if (string.IsNullOrEmpty(secretKey))
    {
      throw new InvalidOperationException("SecretKey is not set up");
    }

    var roles = await _userManager.GetRolesAsync(user);

    byte[]? key = Encoding.UTF8.GetBytes(secretKey);
    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
        new Claim("id", user.Id.ToString()),
        new Claim("username", user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty),
      }),
      Expires = DateTime.UtcNow.AddHours(2),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

    return new UserLoginResponseDto()
    {
      Token = tokenHandler.WriteToken(token),
      User = _mapper.Map<UserDataDto>(user),
      Message = "User logged successfully"
    };
  }

  public async Task<UserDataDto> Register(CreateUserDto createUserDto)
  {
    if (string.IsNullOrEmpty(createUserDto.Username))
    {
      throw new ArgumentException("Username is required");
    }

    if (createUserDto.Password == null)
    {
      throw new ArgumentException("Password is required");
    }

    ApplicationUser user = new ApplicationUser
    {
      UserName = createUserDto.Username,
      Email = createUserDto.Username,
      NormalizedEmail = createUserDto.Username.ToUpper(),
      Name = createUserDto.Name
    };

    IdentityResult result = await _userManager.CreateAsync(user, createUserDto.Password);
    if (result.Succeeded)
    {
      string userRole = createUserDto.Role ?? "User";
      bool roleExists = await _roleManager.RoleExistsAsync(userRole);

      if (!roleExists)
      {
        IdentityRole identityRole = new IdentityRole(userRole);
        await _roleManager.CreateAsync(identityRole);
      }

      await _userManager.AddToRoleAsync(user, userRole);
      ApplicationUser? createdUser = await _db.ApplicationUsers.FirstOrDefaultAsync(user => user.UserName == createUserDto.Username);
      return _mapper.Map<UserDataDto>(createdUser);
    }

    var errors = string.Join(", ", result.Errors.Select(error => error.Description));
    throw new ApplicationException($"Could not create the new register: {errors}");
  }

  // JWT
  private string GenerateJwtToken(User user, string secretKey)
  {
    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

    Claim[] claims = new[]
    {
      new Claim("id", user.Id.ToString()),
      new Claim("user", user.Username),
      new Claim("role", user.Role ?? string.Empty),
    };

    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddHours(2),
      // Issuer = issuer,
      // Audience = audience,
      SigningCredentials = credentials,
    };

    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }
}
