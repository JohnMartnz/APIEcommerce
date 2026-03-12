using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APIEcommerce.Data;
using APIEcommerce.Models;
using APIEcommerce.Models.Dtos;
using APIEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace APIEcommerce.Repository;

public class UserRepository : IUserRepository
{
  private readonly ApplicationDbContext _db;
  private readonly string? secretKey;

  public UserRepository(ApplicationDbContext db, IConfiguration configuration)
  {
    _db = db;
    secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
  }

  public ICollection<User> GetUsers()
  {
    return _db.Users.OrderBy(user => user.Username).ToList();
  }

  public User? GetUser(int id)
  {
    return _db.Users.FirstOrDefault(user => user.Id == id);
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

    User? user = await _db.Users.FirstOrDefaultAsync<User>(user => user.Username.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
    if (user == null)
    {
      return new UserLoginResponseDto()
      {
        Token = "",
        User = null,
        Message = "Username not found"
      };
    }

    if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
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

    byte[]? key = Encoding.UTF8.GetBytes(secretKey);
    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[]
      {
        new Claim("id", user.Id.ToString()),
        new Claim("user", user.Username),
        new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
      }),
      Expires = DateTime.UtcNow.AddHours(2),
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

    return new UserLoginResponseDto()
    {
      Token = tokenHandler.WriteToken(token),
      User = new UserRegisterDto
      {
        Username = user.Username,
        Name = user.Name ?? "No Name",
        Role = user.Role,
        Password = user.Password,
        Id = user.Id.ToString()
      },
      Message = "User logged successfully"
    };
  }

  public async Task<User> Register(CreateUserDto createUserDto)
  {
    string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
    User user = new User()
    {
      Username = createUserDto.Username ?? "No Username",
      Name = createUserDto.Name,
      Role = createUserDto.Role,
      Password = encryptedPassword
    };

    await _db.Users.AddAsync(user);
    await _db.SaveChangesAsync();
    return user;
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
