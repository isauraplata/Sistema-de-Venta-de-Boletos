using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.DTOs.User;
using TicketingSystem.Models;
using TicketingSystem.Interfaces;

namespace TicketingSystem.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IAuthRepository authRepository,
        IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<Response<IdentityResult>> RegisterUser(UserRegisterDTO userDto)
    {
        try
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return new Response<IdentityResult>
                {
                    Success = false,
                    Message = "El correo electrónico ya está registrado",
                    ErrorCode = "EMAIL_EXISTS",
                    Data = null
                };
            }

            var newUser = new User
            {
                UserName = userDto.Username,  // UserName viene de IdentityUser
                Email = userDto.Email,        // Email viene de IdentityUser
                Uuid = Guid.NewGuid(),
                CreateAT = DateTime.UtcNow,
                UpdateAT = DateTime.UtcNow
            };

            var result = await _authRepository.CreateUserAsync(newUser, userDto.Password);
            
            return new Response<IdentityResult>
            {
                Success = result.Succeeded,
                Message = result.Succeeded ? "Usuario registrado exitosamente" : "Error al registrar el usuario",
                Data = result,
                ErrorCode = result.Succeeded ? null : "REGISTRATION_ERROR"
            };
        }
        catch (Exception ex)
        {
            return new Response<IdentityResult>
            {
                Success = false,
                Message = "Error interno del servidor",
                ErrorCode = "SERVER_ERROR",
                Data = null
            };
        }
    }

    public async Task<Response<LoginResponseDTO>> LoginUser(UserLoginDTO loginDto)
    {
        try
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            var normalizedEmail = loginDto.Email.ToUpperInvariant();
            Console.WriteLine("user: " + user);
            Console.WriteLine("normalizedEmail: " + normalizedEmail);
            if (user == null)
            {
                return new Response<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Credenciales inválidas",
                    ErrorCode = "INVALID_CREDENTIALS",
                    Data = null
                };
            }

            var result = await _authRepository.ValidateUserCredentialsAsync(user, loginDto.Password);
            if (!result.Succeeded)
            {
                return new Response<LoginResponseDTO>
                {
                    Success = false,
                    Message = "Credenciales inválidas",
                    ErrorCode = "INVALID_CREDENTIALS",
                    Data = null
                };
            }

            var token = GenerateJwtToken(user);

            return new Response<LoginResponseDTO>
            {
                Success = true,
                Message = "Inicio de sesión exitoso",
                Data = new LoginResponseDTO
                {
                    Id = user.Id,
                    Username = user.UserName,  // Cambiado de Username a UserName
                    Email = user.Email,
                    Token = token
                },
                ErrorCode = null
            };
        }
        catch (Exception ex)
        
        {
            Console.WriteLine("ex "+ex);
            return new Response<LoginResponseDTO>
            {
                Success = false,
                Message = "Error interno del servidor",
                ErrorCode = "SERVER_ERROR",
                Data = null
            };
        }
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),  // Cambiado de Username a UserName
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("uuid", user.Uuid.ToString()),
            new Claim("createAt", user.CreateAT.ToString("yyyy-MM-dd")),
            new Claim("updateAt", user.UpdateAT.ToString("yyyy-MM-dd"))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")
        ));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}